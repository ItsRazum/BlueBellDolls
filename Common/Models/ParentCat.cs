using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    public class ParentCat : Cat, IDisplayableEntity
    {

        #region Properties

        public string GeneticTestOne { get; set; }
        public string GeneticTestTwo { get; set; }
        public List<string> Titles { get; set; }
        public string? OldDescription { get; set; }

        public List<Litter> Litters { get; set; }

        #endregion

        #region Constructor

        public ParentCat()
        {
            Name = "Новый производитель";
            BirthDay = DateOnly.FromDateTime(DateTime.Now);
            IsMale = true;
            Description = "Добавьте описание!";
            Photos = [];
            Titles = [];
            GeneticTestOne = string.Empty;
            GeneticTestTwo = string.Empty;
        }

        #endregion

        #region IDisplayableEntity

        [NotMapped]
        public override string DisplayName => Name;

        #endregion

    }
}
