using MangaHome.Core.Common;
using System.Text.Json.Serialization;

namespace MangaHome.Core.Models;

public class User : BaseEntity
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public Role Role { get; private set; }

    public string? Password { get; private set; }
    public byte[]? Salt { get; private set; }

    public string? ProfilePictureUrl { get; private set; }

    public bool IsBanned { get; set; } = false;

    public bool IsEmailConfirmed { get; set; } = false;
    public string? EmailConfirmationToken { get; private set; }

    [JsonConstructor]
    private User(
        string username,
        string email,
        Role role,
        string password,
        byte[] salt,
        string? profilePictureUrl,
        bool isBanned,
        bool isEmailConfirmed)
    {
        Username = username;
        Email = email;
        Role = role;
        Password = password;
        Salt = salt;
        ProfilePictureUrl = profilePictureUrl;
        IsBanned = isBanned;
        IsEmailConfirmed = isEmailConfirmed;
    }

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

    public void SetPassword(string password, byte[] salt)
    {
        Password = password;
        Salt = salt;
    }

    public void SetProfilePictureUrl(string profilePictureUrl)
    {
        ProfilePictureUrl = profilePictureUrl;
    }

    public void BanUser()
    {
        IsBanned = true;
    }

    public void UnbanUser()
    {
        IsBanned = false;
    }

    public void SetEmailConfirmationToken(string token)
    {
        EmailConfirmationToken = token;
    }

    public void ConfirmEmail(string token)
    {
        if (EmailConfirmationToken == token)
        {
            IsEmailConfirmed = true;
        }
        else
        {
            IsEmailConfirmed = false;
        }
    }
}