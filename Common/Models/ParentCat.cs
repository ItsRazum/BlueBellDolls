using BlueBellDolls.Common.Interfaces.Markers;
using BlueBellDolls.Common.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    [Table("cats")]
    public class ParentCat : Cat, IHandCreatableEntity
    {

        #region IDisplayableEntity

        [NotMapped]
        public override string DisplayName => Name;

        #endregion

    }
}
