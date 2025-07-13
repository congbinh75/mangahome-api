using System.ComponentModel.DataAnnotations.Schema;

namespace MangaHome.Core.Models;

public class BaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid? Id { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}