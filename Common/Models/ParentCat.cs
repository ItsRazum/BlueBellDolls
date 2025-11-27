using BlueBellDolls.Common.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    [Table("cats")]
    public class ParentCat : Cat
    {

        #region IDisplayableEntity

        [NotMapped]
        public override string DisplayName => Name;

        #endregion

    }
}
