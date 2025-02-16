using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface ICallbackDataProvider
    {
        string CreateConfirmCallback(string baseCallback);
        string CreateEditEntityCallback(IDisplayableEntity entity);
        string CreateSelectParentCatCallback(bool isMale, int litterId);
        string CreateOpenEntityInLitterCallback(IEntity entity, int litterId);
        string CreateAddKittenToLitterCallback(int litterId);
        string CreateBackToLitterCallback(int litterId);
        string CreateListEntityCallback(string entityName, int page);
        string CreateDeleteEntityCallback(IDisplayableEntity entity, int fromLitterId = 0);
        string CreateDeleteParentCatFromLitterCallback(int parentCatId, int litterId);
        string CreateEntityReferenceCallback(IDisplayableEntity entity, ListUnitActionMode actionMode, IEntity? unitOwner = null);
        string CreateAddEntityCallback(string entityName);
        string CreateAddPhotosCallback(IDisplayableEntity entity);
        string CreateAddTitlesCallback(IDisplayableEntity entity);
    }
}
