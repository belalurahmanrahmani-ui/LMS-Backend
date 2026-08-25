namespace LMS.Helpers
{
    public interface IPasswordHasher
    {
        string HasPassword(string password);
        bool verifyPassword(string password, string passwordHash);
    }
}
