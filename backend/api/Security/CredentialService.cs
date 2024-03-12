using System.Security.Cryptography;
using System.Text;
using Serilog;

namespace api.Security;

public class CredentialService
{
    public string GenerateSalt(int size = 32)
    {
        // Create a buffer
        var randomBuffer = new byte[size];

        using (var rng = RandomNumberGenerator.Create())
        {
            // Fill the buffer with random data
            rng.GetBytes(randomBuffer);
        }

        // Convert the bytes to a Base64 string
        var salt = Convert.ToBase64String(randomBuffer);

        return salt;
    }

    public string Hash(string password, string salt)
    {
        try
        {
            using var sha256 = SHA256.Create();
            // Compute hash of the password concatenated with the salt
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltBytes = Convert.FromBase64String(salt);
            var passwordWithSaltBytes = new byte[passwordBytes.Length + saltBytes.Length];
            Buffer.BlockCopy(passwordBytes, 0, passwordWithSaltBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(saltBytes, 0, passwordWithSaltBytes, passwordBytes.Length, saltBytes.Length);
            var hashBytes = sha256.ComputeHash(passwordWithSaltBytes);
            // Convert the hash bytes to a Base64 string
            var hash = Convert.ToBase64String(hashBytes);
            return hash;
        }
        catch (Exception e)
        {
            Log.Error(e, "Hashing error");
            throw new InvalidOperationException("Hashing failed");
        }
    }


}