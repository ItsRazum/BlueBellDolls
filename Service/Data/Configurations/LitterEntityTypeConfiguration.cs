using BlueBellDolls.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueBellDolls.Service.Data.Configurations
{
    internal class LitterEntityTypeConfiguration : IEntityTypeConfiguration<Litter>
    {
        public void Configure(EntityTypeBuilder<Litter> builder)
        {
            builder
                .HasOne(l => l.FatherCat)
                .WithMany()
                .HasForeignKey(l => l.FatherCatId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(l => l.MotherCat)
                .WithMany()
                .HasForeignKey(l => l.MotherCatId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(l => l.Kittens)
                .WithOne(k => k.Litter)
                .HasForeignKey(k => k.LitterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
