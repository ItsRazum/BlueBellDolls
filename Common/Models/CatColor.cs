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
        public string? Description { get; set; }

        #region IDisplayableEntity

        public override string DisplayName => Identifier.Humanize(LetterCasing.Title);

        #endregion

        #region ICanBeFoundWithName

        public string QueryName => Identifier;

        #endregion

        public override bool ReadyToShow => this is 
        {
            Identifier.Length: > 0,
            Description.Length: > 0,
            Photos.Count: > 0
        };
    }
}
