namespace Service.Security.PasswordHasher;

public interface IPasswordHasher
{
    string Hash(string password, byte[] salt);
}