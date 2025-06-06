﻿using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Static;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueBellDolls.Common.Data.Configurations
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
                .Property(k => k.Photos)
                .HasConversion(ValueConverters.DictionaryStringConverter)
                .Metadata
                .SetValueComparer(ValueComparers.DictionaryStringComparer);
        }
    }
}
