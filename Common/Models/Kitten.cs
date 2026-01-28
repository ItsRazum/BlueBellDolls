using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    [Table("kittens")]
    public class Kitten : Cat
    {

        #region Properties

        [Column("class")]
        public KittenClass Class { get; set; }

        [Column("status")]
        public KittenStatus Status { get; set; }

        [Column("litter_id")]
        [ForeignKey(nameof(Litter))]
        public int LitterId { get; set; }
        public Litter Litter { get; set; }

        #endregion

        #region IDisplayableEntity

        public override string DisplayName => Name ?? string.Empty;

        public override bool ReadyToShow => this is 
        {
            Name.Length: > 0,
            Description.Length: > 0,
            Photos.Count: > 0,
            CatColorId: not null
        };

        #endregion

    }
}