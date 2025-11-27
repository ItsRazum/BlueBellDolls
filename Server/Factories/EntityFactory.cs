using BlueBellDolls.Common.Models;
using BlueBellDolls.Server.Interfaces;

namespace BlueBellDolls.Server.Factory
{
    public class EntityFactory : IEntityFactory
    {
        public ParentCat CreateNewParentCat(
            string name = "Новый производитель",
            bool isMale = true,
            string description = "Добавьте описание!",
            string color = "Не указан")
        {
            return new ParentCat
            {
                Name = name,
                IsMale = isMale,
                Description = description,
                Color = color,
                Photos = [],
            };
        }

        public Kitten CreateNewKitten(
            string name = "Новый производитель",
            bool isMale = true,
            string description = "Добавьте описание!",
            string color = "Не указан")
        {
            return new Kitten
            {
                Name = name,
                IsMale = isMale,
                Description = description,
                Color = color,
                Photos = [],
                Status = Common.Enums.KittenStatus.Available,
                Class = Common.Enums.KittenClass.Pet
            };
        }

        public Litter CreateNewLitter(
            string description = "Добавьте описание!")
        {
            return new Litter
            {
                Description = description,
                Photos = [],
            };
        }
    }
}
