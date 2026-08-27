using LMS.Data;
using LMS.DTOs;
using LMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services
{
    public class UserService : IUserService
    {
        private readonly LmsDbContext _dbContext;
        public UserService(LmsDbContext context)
        {
            _dbContext = context;
        }

        public async Task<bool> ChangeRoleAsync(int id, UserRole newRole)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return false;
            user.Role = newRole;
            await _dbContext.SaveChangesAsync();
            return true;
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var usre = await _dbContext.Users.FindAsync(id);
            if (usre == null)
                return false;
            _dbContext.Users.Remove(usre);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserResponseDto>> GetAllUserAsync()
        {
            return await _dbContext.Users
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    ProfileImag = u.ProfileImage,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                }).ToListAsync();


        }

        

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            return await _dbContext.Users
                .Where(u => u.Id == id)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    ProfileImag = u.ProfileImage,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                }).FirstOrDefaultAsync();
            
            
        }
        // Note:
        public async Task<bool> SetActiveStatusAsync(int id, bool isActive)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if(user == null) 
                return false;
            user.IsActive = isActive;
            await _dbContext.SaveChangesAsync();
            return true;

            
        }

        public async Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) 
                return null;
            bool EmailTaken = await _dbContext.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower() && u.Id != id);
            if (EmailTaken)
            {
                throw new InvalidOperationException("Email is already taken or user by another user.");
            }
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.ProfileImage = dto.ProfileImage;
            await _dbContext.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                ProfileImag = user.ProfileImage,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
