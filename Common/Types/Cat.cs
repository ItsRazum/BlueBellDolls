using BlueBellDolls.Common.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Types
{
    public abstract class Cat : DisplayableEntityBase
    {

        #region Properties

        [Column("name")]
        public string? Name { get; set; }

        [Column("birthday")]
        public DateOnly BirthDay { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Column("is_male")]
        public bool IsMale { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("cat_color_id")]
        public int? CatColorId { get; set; }
        public CatColor? Color { get; set; }

        #endregion

    }
}
