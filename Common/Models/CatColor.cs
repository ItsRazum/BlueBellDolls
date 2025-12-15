using BlueBellDolls.Common.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    [Table("cat_colors")]
    public class CatColor : DisplayableEntityBase
    {
        [Column("identifier")]
        public string Identifier { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [NotMapped]
        public override string DisplayName => Identifier;
    }
}
