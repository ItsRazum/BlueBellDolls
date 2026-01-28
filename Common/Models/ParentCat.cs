using BlueBellDolls.Common.Interfaces.Markers;
using BlueBellDolls.Common.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    [Table("cats")]
    public class ParentCat : Cat, IHandCreatableEntity
    {

        #region IDisplayableEntity

        [NotMapped]
        public override string DisplayName => Name ?? string.Empty;

        public override bool ReadyToShow 
        { 
            get
            {
                var photos = Photos.Where(p => p.Type == Enums.PhotosType.Photos);
                var titles = Photos.Where(p => p.Type == Enums.PhotosType.Titles);
                var genTests = Photos.Where(p => p.Type == Enums.PhotosType.GenTests);

                return this is
                {
                    Name.Length: > 0,
                    Description.Length: > 0,
                    CatColorId: not null
                } && photos.Any() && titles.Any() && genTests.Any();
            } 
        }

        #endregion

    }
}
