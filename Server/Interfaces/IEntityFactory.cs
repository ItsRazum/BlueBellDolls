using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Server.Interfaces
{
    public interface IEntityFactory
    {
        Kitten CreateNewKitten(string name = "Новый производитель", bool isMale = true, string description = "Добавьте описание!", string color = "Не указан");
        Litter CreateNewLitter(string description = "Добавьте описание!");
        ParentCat CreateNewParentCat(string name = "Новый производитель", bool isMale = true, string description = "Добавьте описание!", string color = "Не указан");
    }
}