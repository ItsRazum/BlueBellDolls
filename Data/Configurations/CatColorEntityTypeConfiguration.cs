using BlueBellDolls.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueBellDolls.Data.Configurations
{
    internal class CatColorEntityTypeConfiguration : IEntityTypeConfiguration<CatColor>
    {
        public void Configure(EntityTypeBuilder<CatColor> builder)
        {
            builder
                .HasIndex(c => c.Identifier)
                .IsUnique();

            builder
                .Property(c => c.Identifier)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
