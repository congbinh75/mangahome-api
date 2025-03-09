namespace MangaHome.Api.Models.Dtos;

public class UserDto
{
    public string? Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public bool? IsEmailConfirmed { get; set; }
    public string? ProfilePicture { get; set; }
    public int? Role { get; set; }
    public bool? IsBanned { get; set; }
}