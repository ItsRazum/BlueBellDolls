using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    [Table("photos")]
    public class EntityPhoto : EntityBase
    {

        [Column("url")]
        public string Url { get; set; }

        [Column("is_main")]
        public bool IsMain { get; set; }

        [Column("type")]
        public PhotosType Type { get; set; }

        [Column("parent_cat_id")]
        [ForeignKey(nameof(ParentCat))]
        public int? ParentCatId { get; set; }
        public ParentCat? ParentCat { get; set; }

        [Column("kitten_id")]
        [ForeignKey(nameof(Kitten))]
        public int? KittenId { get; set; }
        public Kitten? Kitten { get; set; }

        [Column("litter_id")]
        [ForeignKey(nameof(Litter))]
        public int? LitterId { get; set; }
        public Litter? Litter { get; set; }

        public TelegramPhoto? TelegramPhoto { get; set; }
    }
}
