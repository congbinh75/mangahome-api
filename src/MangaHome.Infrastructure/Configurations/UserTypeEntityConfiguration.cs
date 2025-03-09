using MangaHome.Core.Common;
using MangaHome.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaHome.Infrastructure.Configurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasIndex(b => new { b.Username, b.Email })
            .IsUnique();

        builder
            .Property(x => x.Id)
            .IsRequired();

        builder
            .Property(x => x.Username)
            .HasMaxLength(Constants.UsernameMaxLength)
            .IsRequired();
        
        builder
            .Property(x => x.Email)
            .IsRequired();
        
        builder
            .Property(x => x.Role)
            .IsRequired();

        builder
            .Property(x => x.Password)
            .IsRequired();

        builder
            .Property(x => x.Salt)
            .IsRequired();
        
        builder
            .Property(x => x.IsBanned)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder
            .Property(x => x.IsEmailConfirmed)
            .IsRequired()
            .HasDefaultValue(false);
    }
}