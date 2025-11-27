using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Types
{
    public abstract class Cat : DisplayableEntityBase
    {

        #region Properties

        [Column("name")]
        public string Name { get; set; }

        [Column("birthday")]
        public DateOnly BirthDay { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Column("is_male")]
        public bool IsMale { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("color")]
        public string Color { get; set; }

        #endregion

    }
}
