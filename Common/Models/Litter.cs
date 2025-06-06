﻿using BlueBellDolls.Common.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    public class Litter : DisplayableEntityBase
    {

        #region Properties

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
            Photos = [];
        }

        #endregion


        #region IDisplayableEntity implementation

        [NotMapped]
        public override string DisplayName => "Помёт " + Letter;

        #endregion

    }
}
