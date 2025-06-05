using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    public class Kitten : Cat
    {

        #region Properties

        public KittenClass Class { get; set; }
        public KittenStatus Status { get; set; }

        [ForeignKey(nameof(Litter))]
        public int LitterId { get; set; }
        public Litter Litter { get; set; }

        #endregion

        #region Constructor

        public Kitten()
        {
            Name = "Новый котёнок";
            BirthDay = DateOnly.FromDateTime(DateTime.Now);
            IsMale = true;
            Description = "Добавьте описание!";
            Color = "Не указан";
            Photos = [];
            Class = KittenClass.Pet;
            Status = KittenStatus.Available;
        }

        #endregion

        #region IDisplayableEntity

        [NotMapped]
        public override string DisplayName => Name;

        #endregion

    }
}