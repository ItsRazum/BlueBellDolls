using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Static;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueBellDolls.Common.Data.Configurations
{
    internal class ParentCatEntityTypeConfiguration : IEntityTypeConfiguration<ParentCat>
    {
        public void Configure(EntityTypeBuilder<ParentCat> builder)
        {
            builder
                .Property(c => c.Titles)
                .HasConversion(ValueConverters.DictionaryStringConverter)
                .Metadata
                .SetValueComparer(ValueComparers.DictionaryStringComparer);

            builder
                .Property(c => c.Photos)
                .HasConversion(ValueConverters.DictionaryStringConverter)
                .Metadata
                .SetValueComparer(ValueComparers.DictionaryStringComparer);
        }
    }
}
