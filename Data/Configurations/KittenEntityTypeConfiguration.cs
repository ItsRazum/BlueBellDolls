using BlueBellDolls.Common.Models;
using BlueBellDolls.Data.Static;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueBellDolls.Data.Configurations
{
    public class KittenEntityTypeConfiguration : IEntityTypeConfiguration<Kitten>
    {
        public void Configure(EntityTypeBuilder<Kitten> builder)
        {
            builder
                .Property(k => k.Class)
                .HasConversion<string>();

            builder
                .Property(k => k.Status)
                .HasConversion<string>();

            builder
                .HasMany(k => k.Photos)
                .WithOne(p => p.Kitten)
                .HasForeignKey(p => p.KittenId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
