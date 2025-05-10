namespace MangaHome.Api.Models.ViewModels;

public class UserViewModel
{
    public string? Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public bool? IsEmailConfirmed { get; set; }
    public string? ProfilePicture { get; set; }
    public int? Role { get; set; }
    public bool? IsBanned { get; set; }
}