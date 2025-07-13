using MangaHome.Core.Values;

namespace MangaHome.Core.Models;

public class User : BaseEntity
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public Role Role { get; private set; }

    public string? Password { get; private set; }
    public byte[]? Salt { get; private set; }

    internal User() { }

    public User(
        string username,
        string email,
        Role role,
        string password,
        byte[] salt
    )
    {
        Username = username;
        Email = email;
        Role = role;
        Password = password;
        Salt = salt;
    }
}