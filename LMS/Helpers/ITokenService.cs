using LMS.Entities;
using Microsoft.Extensions.Options;

namespace LMS.Helpers
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
       
        string GenerateRefreshToken();
    }
}
