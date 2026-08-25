using Microsoft.IdentityModel.Tokens;

namespace LMS.Helpers
{
    public class PasswordHasher : IPasswordHasher
        
    {
        // workingFactor controls how computatonally expensive the hash is 
        // 12 is a solid default : secure enough, still fast enough for a web request.
        private const int WorkFactor = 12;
        public string HasPassword(string password)
        {
            // BCrypt automatically generates a ranop salt and embeds is 
            // inside the returned hash string . we don't need to stor salt separate.
            return BCrypt.Net.BCrypt.HashPassword(password,WorkFactor);
            
        }

        public bool verifyPassword(string password, string passwordHash)
        {
            // BCrypt extracts the salt from passwordHash itself
            // hashes the given password with that salt, and compares.
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
