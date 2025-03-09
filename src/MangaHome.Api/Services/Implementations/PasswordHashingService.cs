using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace MangaHome.Api.Services.Implementations;

public class PasswordHashingService : IPasswordHashingService
{
    private readonly ILogger<PasswordHashingService> _logger;

    public PasswordHashingService(ILogger<PasswordHashingService> logger)
    {
        _logger = logger;
    }

    public (string hashedPassword, byte[] salt) HashPassword(string password, byte[]? salt = null)
    {
        salt ??= RandomNumberGenerator.GetBytes(128 / 8);
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

        _logger.LogInformation("Requested service: {ServiceName}; Method: {MethodName}; Input: {Input}; Output: {Output}",
            this.GetType().Name,
            nameof(HashPassword),
            null,
            null);
        
        return (hashed, salt);
    }
}