using System.ComponentModel.DataAnnotations;

namespace MangaHome.Core.Values;

public enum Role
{
    [Display(Name = "Admin")]
    Admin,
    [Display(Name = "Moderator")]
    Moderator,
    [Display(Name = "User")]
    User
}
