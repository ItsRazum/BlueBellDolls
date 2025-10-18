using BlueBellDolls.Common.Models;
using BlueBellDolls.Data.Static;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueBellDolls.Data.Configurations
{
    public class LitterEntityTypeConfiguration : IEntityTypeConfiguration<Litter>
    {
        public void Configure(EntityTypeBuilder<Litter> builder)
        {
            builder
                .HasOne(l => l.FatherCat)
                .WithMany()
                .HasForeignKey(l => l.FatherCatId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(l => l.MotherCat)
                .WithMany()
                .HasForeignKey(l => l.MotherCatId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(l => l.Kittens)
                .WithOne(k => k.Litter)
                .HasForeignKey(k => k.LitterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Property(l => l.Photos)
                .HasConversion(ValueConverters.DictionaryStringConverter)
                .Metadata
                .SetValueComparer(ValueComparers.DictionaryStringComparer);
        }
    }
}
