using System.ComponentModel.DataAnnotations;

namespace MangaHome.Api.Common;

public enum ErrorType
{
    [Display(Name = "Validation")]
    Validation = 0,
    [Display(Name = "Error")]
    Error = 1,
}
