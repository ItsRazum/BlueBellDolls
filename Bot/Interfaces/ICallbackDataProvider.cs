using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Types;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface ICallbackDataProvider
    {
        string GetAddKittenToLitterCallback();
        string GetAddEntityCallback<T>() where T : IDisplayableEntity;
        string GetConfirmDeleteEntityCallback<T>() where T : IDisplayableEntity;
        string GetConfirmDeletePhotoCallback<T>(PhotosType mode) where T : IDisplayableEntity;
        string GetDeleteEntityCallback<T>() where T : IDisplayableEntity;
        string GetDeletePhotoCallback<T>(PhotosType mode) where T : IDisplayableEntity;
        string GetDeleteMessagesCallback();
        string GetEditEntityCallback<T>() where T : IDisplayableEntity;
        string GetListEntityCallback<T>() where T : IDisplayableEntity;
        string GetManagePhotosCallback<T>(PhotosType mode) where T : IDisplayableEntity;
        string GetOpenEntityCallback<T>() where T : IDisplayableEntity;
        string GetSelectEntityCallback<T>() where T : IDisplayableEntity;
        string GetSelectToLitterCallback();
        string GetSetDefaultPhotoCallback<T>(PhotosType mode) where T : IDisplayableEntity;
        string GetTogglePhotoSelectionCallback<T>() where T : IDisplayableEntity;
        string GetFindColorCallback<T>() where T : Cat;
        string GetToggleEntityVisibilityCallback<T>() where T : IDisplayableEntity;
        string GetOpenKittenStatus();
        string GetOpenKittenClass();
        string GetSetKittenStatus();
        string GetSetKittenClass();

        string CreateConfirmCallback(string baseCallback);
        string CreateEditEntityCallback(IDisplayableEntity entity);
        string CreateSelectParentCatCallback(bool isMale, int page, int litterId);
        string CreateOpenEntityInLitterCallback(IEntity entity, int litterId);
        string CreateAddKittenToLitterCallback(int litterId);
        string CreateBackToLitterCallback(int litterId);
        string CreateListEntityCallback(string entityName, int page);
        string CreateDeleteEntityCallback(IDisplayableEntity entity, int fromLitterId = 0);
        string CreateEntityReferenceCallback(IDisplayableEntity entity, ListUnitActionMode actionMode, IEntity? unitOwner = null);
        string CreateAddEntityCallback(string entityName);
        string CreateManagePhotosCallback(IDisplayableEntity entity, PhotosType photosType);
        string CreateTogglePhotoSelectionCallback(IDisplayableEntity entity, int photoId, bool select, PhotosType photosType);
        string CreateMakeDefaultPhotoForEntityCallback(IDisplayableEntity entity, int photoId, PhotosType photosType);
        string CreateDeletePhotosForEntityCallback(IDisplayableEntity entity, PhotosType photosType);
        string CreateDeleteMessagesCallback(int[] messagesId);
        string CreateDeleteMessagesCallback();
        string CreateFindColorCallback(Cat entity, string buildedColor, string colorPart);
        string CreateStartFindColorCallback(Cat entity);
        string CreateToggleEntityVisibilityCallback(IDisplayableEntity entity);
        string CreateOpenKittenClass(int kittenId);
        string CreateOpenKittenStatus(int kittenId);
        string CreateSetKittenClass(int kittenId, KittenClass kittenClass);
        string CreateSetKittenStatus(int kittenId, KittenStatus kittenStatus);
    }
}
