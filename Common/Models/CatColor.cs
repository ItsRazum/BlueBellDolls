using BlueBellDolls.Common.Interfaces.Markers;
using BlueBellDolls.Common.Types;
using Humanizer;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    [Table("cat_colors")]
    public class CatColor : DisplayableEntityBase, ICanBeFoundWithName
    {
        [Column("identifier")]
        public string Identifier { get; set; }

        [Column("description")]
        public string Description { get; set; }

        public override string DisplayName => Identifier.Humanize(LetterCasing.Title);

        public string QueryName => Identifier;
    }
}
