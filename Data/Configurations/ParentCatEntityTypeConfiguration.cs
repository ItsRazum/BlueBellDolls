using BlueBellDolls.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueBellDolls.Data.Configurations
{
    internal class ParentCatEntityTypeConfiguration : IEntityTypeConfiguration<ParentCat>
    {
        public void Configure(EntityTypeBuilder<ParentCat> builder)
        {
            builder
                .HasMany(c => c.Photos)
                .WithOne(p => p.ParentCat)
                .HasForeignKey(p => p.ParentCatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
