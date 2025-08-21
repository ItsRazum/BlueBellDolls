using BlueBellDolls.Common.Models;
using BlueBellDolls.Data.Static;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueBellDolls.Data.Configurations
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

            builder
                .Property(c => c.GeneticTests)
                .HasConversion(ValueConverters.DictionaryStringConverter)
                .Metadata
                .SetValueComparer(ValueComparers.DictionaryStringComparer);
        }
    }
}
