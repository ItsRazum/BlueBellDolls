using BlueBellDolls.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace BlueBellDolls.Data.Configurations
{
    internal class EntityPhotoEntityTypeConfiguration : IEntityTypeConfiguration<EntityPhoto>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<EntityPhoto> builder)
        {
            builder.HasOne(ep => ep.TelegramPhoto)
                   .WithOne(tp => tp.EntityPhoto)
                   .HasForeignKey<TelegramPhoto>(tp => tp.EntityPhotoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
