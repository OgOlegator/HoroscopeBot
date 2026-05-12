using HoroscopeBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoroscopeBot.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TelegramId).IsRequired();
        builder.HasIndex(x => x.TelegramId).IsUnique();

        builder.Property(x => x.UserName).HasMaxLength(100);
        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.BirthCity).HasMaxLength(200);
        builder.Property(x => x.TimeZone).HasMaxLength(100);

        builder.Property(x => x.ZodiacSign)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.BirthDate).IsRequired();
    }
}
