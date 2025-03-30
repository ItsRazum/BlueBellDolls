using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    public class ParentCat : Cat, IDisplayableEntity
    {

        #region Properties

        public Dictionary<string, string> GeneticTests { get; set; }
        public Dictionary<string, string> Titles { get; set; }
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
            Color = "Не указан";
            Photos = [];
            Titles = [];
            GeneticTests = [];
        }

        #endregion

        #region IDisplayableEntity

        [NotMapped]
        public override string DisplayName => Name;

        #endregion

    }
}
