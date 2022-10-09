using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Service.Security.PasswordHasher;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password, byte[] salt)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA1,
            10000,
            256 / 8));
    }
}