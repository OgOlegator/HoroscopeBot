using HoroscopeBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HoroscopeBot.Infrastructure.Data.Configurations;

public class HoroscopeConfiguration : IEntityTypeConfiguration<Horoscope>
{
    public void Configure(EntityTypeBuilder<Horoscope> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Text).IsRequired();

        builder.Property(x => x.AiProvider)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Date)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Horoscopes)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.Date });
    }
}
