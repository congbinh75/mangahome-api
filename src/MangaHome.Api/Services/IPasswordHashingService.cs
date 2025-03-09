namespace MangaHome.Api.Services;

public interface IPasswordHashingService
{
    (string hashedPassword, byte[] salt) HashPassword(string password, byte[]? salt = null);
}