using BlueBellDolls.Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    public class Litter : IDisplayableEntity
    {

        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public char Letter { get; set; }
        public DateOnly BirthDay { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey(nameof(MotherCat))]
        public int? MotherCatId { get; set; }
        public ParentCat? MotherCat { get; set; }

        [ForeignKey(nameof(FatherCat))]
        public int? FatherCatId { get; set; }
        public ParentCat? FatherCat { get; set; }

        public List<Kitten> Kittens { get; set; }
        public string Description { get; set; }

        #endregion

        #region Constructor 

        public Litter()
        {
            Letter = 'A';
            BirthDay = DateOnly.FromDateTime(DateTime.Now);
            IsActive = true;
            Description = "Добавьте описание!";
            Kittens = [];
        }

        #endregion


        #region IDisplayableEntity implementation

        [NotMapped]
        public string DisplayName => "Помёт " + Letter;

        #endregion

    }
}
