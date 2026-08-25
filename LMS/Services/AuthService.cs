using LMS.Data;
using LMS.DTOs.Auth;
using LMS.Entities;
using LMS.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace LMS.Services
{
    public class AuthService : IAuthService
    {
        private readonly LmsDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;
        public AuthService (
            LmsDbContext context,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IOptions<JwtSettings> jwtSettings
            )
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
        }
        public async Task<AuthResponseDto> LoginAsyn(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if ( user == null )
            {
                throw new UnauthorizedAccessException("Invalid Email Or password");
            }
            if (!user.IsActive)
                throw new UnauthorizedAccessException("This acount has been deactivated");
            var passwordValid = _passwordHasher.verifyPassword(dto.Password, user.PasswordHash);
            if (!passwordValid)
                throw new UnauthorizedAccessException("Invalid Email Or password.");
            return await GenerateAuthResponsAsyn(user);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);
            // Sam principle as login : don't reveal WHY it failed 
            // (dosn't exist vs expired vs revoked) - just reject it .
            if (storedToken == null)
                throw new UnauthorizedAccessException("Invalid Refresh token");
            if (storedToken.IsRevoked)
                throw new UnauthorizedAccessException("Refresh token has been revoked ");
            if (storedToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token has expired");
            storedToken.IsRevoked = true;
            await _context.SaveChangesAsync();

            return await GenerateAuthResponsAsyn(storedToken.User);
        }
        public async Task LogoutAsync(int userid)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userid && !rt.IsRevoked).ToListAsync();
            foreach(var token in activeTokens)
            {
                token.IsRevoked = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var emailExists = await _context.Users
               .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());
            if (emailExists)
            {
                throw new InvalidCastException("Email is already registerd.");
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = _passwordHasher.HasPassword(dto.Password),
                Role = UserRole.Student,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return await GenerateAuthResponsAsyn(user);
        }
        private async Task<AuthResponseDto> GenerateAuthResponsAsyn(User user)
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshTokenValue = _tokenService.GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                Token = refreshTokenValue,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreateAt = DateTime.UtcNow,
                IsRevoked = false
            };
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)
            };
        }
    }
}
