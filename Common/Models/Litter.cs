using BlueBellDolls.Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    public class Litter : IEntity
    {

        #region Properties

        [Key]
        public int Id { get; set; }
        public char Letter { get; set; }
        public DateOnly BirthDay { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey(nameof(MotherCat))]
        public int MotherCatId { get; set; }
        public ParentCat MotherCat { get; set; }

        [ForeignKey(nameof(FatherCat))]
        public int FatherCatId { get; set; }
        public ParentCat FatherCat { get; set; }

        public List<Kitten> Kittens { get; set; }
        public string Description { get; set; }

        #endregion

    }
}
