using BlueBellDolls.Common.Interfaces.Markers;
using BlueBellDolls.Common.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    [Table("litters")]
    public class Litter : DisplayableEntityBase, IHandCreatableEntity
    {

        #region Properties

        [Column("letter")]
        public char Letter { get; set; }

        [Column("birth_day")]
        public DateOnly BirthDay { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Column("mother_cat_id")]
        [ForeignKey(nameof(MotherCat))]
        public int? MotherCatId { get; set; }
        public ParentCat? MotherCat { get; set; }

        [Column("father_cat_id")]
        [ForeignKey(nameof(FatherCat))]
        public int? FatherCatId { get; set; }
        public ParentCat? FatherCat { get; set; }

        public List<Kitten> Kittens { get; set; }

        [Column("description")]
        public string Description { get; set; }

        #endregion

        #region IDisplayableEntity implementation

        [NotMapped]
        public override string DisplayName => "Помёт " + Letter;

        #endregion

    }
}
