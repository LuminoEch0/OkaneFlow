namespace Service.Interface
{
    public interface IPasswordManager
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string storedHash);
    }
}
