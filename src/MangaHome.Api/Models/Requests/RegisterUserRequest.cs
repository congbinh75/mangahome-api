namespace MangaHome.Api.Models.Requests;

public class RegisterUserRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
}