using BlueBellDolls.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueBellDolls.Service.Data.Configurations
{
    internal class KittenEntityTypeConfiguration : IEntityTypeConfiguration<Kitten>
    {
        public void Configure(EntityTypeBuilder<Kitten> builder)
        {
            builder
                .Property(k => k.Class)
                .HasConversion<string>();

            builder
                .Property(k => k.Status)
                .HasConversion<string>();
        }
    }
}
