using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Settings;
using Microsoft.Extensions.Options;
using BlueBellDolls.Common.Types;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Bot.Interfaces.Providers;

namespace BlueBellDolls.Bot.Providers
{
    public class CallbackDataProvider : ICallbackDataProvider
    {

        private readonly CallbackCommandIdentifiers _settings;
        
        private string Separator { get; }

        public CallbackDataProvider(
            IOptions<BotSettings> botSettings) 
        {
            var callbackDataSettings = botSettings.Value.CallbackDataSettings;

            _settings = callbackDataSettings.CallbackCommandIdentifiers;
            Separator = callbackDataSettings.ArgsSeparator;
        }

        #region Get Methods

        public string GetAddKittenToLitterCallback() 
            => _settings.AddKittenToLitter;

        public string GetAddEntityCallback<T>() where T : IDisplayableEntity
            => $"{_settings.AddEntity}{typeof(T).Name}";

        public string GetConfirmDeleteEntityCallback<T>() where T : IDisplayableEntity
            => $"{_settings.ConfirmationSuffix}{_settings.DeleteEntity}{typeof(T).Name}";

        public string GetConfirmDeletePhotoCallback<T>(PhotosType mode) where T : IDisplayableEntity
            => $"{_settings.ConfirmationSuffix}{_settings.DeletePhoto}{mode}For{typeof(T).Name}";

        public string GetDeleteEntityCallback<T>() where T : IDisplayableEntity
            => $"{_settings.DeleteEntity}{typeof(T).Name}";

        public string GetDeletePhotoCallback<T>(PhotosType mode) where T : IDisplayableEntity
            => $"{_settings.DeletePhoto}{mode}For{typeof(T).Name}";

        public string GetDeleteMessagesCallback() => _settings.DeleteMessages;

        public string GetEditEntityCallback<T>() where T : IDisplayableEntity
            => $"{_settings.EditEntity}{typeof(T).Name}";

        public string GetListEntityCallback<T>() where T : IDisplayableEntity
            => $"{_settings.ListEntity}{typeof(T).Name}";

        public string GetManagePhotosCallback<T>(PhotosType mode) where T : IDisplayableEntity
            => $"{_settings.ManagePhotos}{mode}To{typeof(T).Name}";

        public string GetOpenEntityCallback<T>() where T : IDisplayableEntity
            => $"{_settings.OpenEntity}{typeof(T).Name}";

        public string GetSelectEntityCallback<T>() where T : IDisplayableEntity
            => $"{_settings.SelectEntity}{typeof(T).Name}";

        public string GetSelectToLitterCallback()
            => _settings.SelectToLitter;

        public string GetSetDefaultPhotoCallback<T>(PhotosType mode) where T : IDisplayableEntity
            => $"{_settings.SetDefaultPhoto}{mode}For{typeof(T).Name}";

        public string GetTogglePhotoSelectionCallback<T>() where T : IDisplayableEntity
            => $"{_settings.TogglePhotoSelection}{typeof(T).Name}";

        public string GetFindColorCallback<T>() where T : Cat
            => $"{_settings.FindColor}{typeof(T).Name}";

        public string GetToggleEntityVisibilityCallback<T>() where T : IDisplayableEntity
            => $"{_settings.ToggleEntityVisibility}{typeof(T).Name}";

        public string GetOpenKittenStatusCallback()
            => _settings.OpenKittenStatus;

        public string GetOpenKittenClassCallback()
            => _settings.OpenKittenClass;

        public string GetSetKittenStatusCallback()
            => _settings.SetKittenStatus;

        public string GetSetKittenClassCallback()
            => _settings.SetKittenClass;

        #endregion

        #region Create methods

        public string CreateConfirmCallback(string baseCallback)
            => $"{_settings.ConfirmationSuffix}{baseCallback}";

        public string CreateEditEntityCallback(IDisplayableEntity entity)
            => $"{_settings.EditEntity}{entity.GetType().Name}{Separator}{entity.Id}";

        public string CreateSelectParentCatCallback(bool isMale, int page, int litterId)
            => $"{_settings.SelectEntity}{nameof(ParentCat)}{Separator}{isMale}{Separator}{page}{Separator}{litterId}";

        public string CreateOpenEntityInLitterCallback(IEntity entity, int litterId)
            => $"{_settings.OpenEntity}{entity.GetType().Name}{Separator}{litterId}{Separator}{entity.Id}";

        public string CreateAddKittenToLitterCallback(int litterId)
            => $"{_settings.AddKittenToLitter}{Separator}{litterId}";

        public string CreateBackToLitterCallback(int litterId)
            => $"{_settings.EditEntity}{nameof(Litter)}{Separator}{litterId}";

        public string CreateListEntityCallback(string entityName, int page)
            => $"{_settings.ListEntity}{entityName}{Separator}{page}";

        public string CreateDeleteEntityCallback(IDisplayableEntity entity, int fromLitterId = 0)
            => $"{_settings.DeleteEntity}{entity.GetType().Name}{(fromLitterId != 0 ? $"{Separator}fromLitter{Separator}{fromLitterId}" : string.Empty)}{Separator}{entity.Id}";

        public string CreateEntityReferenceCallback(IDisplayableEntity entity, ListUnitActionMode actionMode, IEntity? unitOwner = null)
        {
            var entityType = entity.GetType().Name;
            if (unitOwner == null)
            {
                return $"{actionMode.ToString().ToLower()}{entityType}{Separator}{entity.Id}";
            }
            else
            {
                if (actionMode != ListUnitActionMode.Select)
                    throw new ArgumentException("При указанном владельце сущности режим работы может быть только Select!");

                return $"{_settings.SelectToLitter}{Separator}{unitOwner.Id}{Separator}{entityType}{Separator}{entity.Id}";
            }
        }

        public string CreateAddEntityCallback(string entityName)
            => $"{_settings.AddEntity}{entityName}";

        public string CreateManagePhotosCallback(IDisplayableEntity entity, PhotosType photosManagementMode)
            => $"{_settings.ManagePhotos}{photosManagementMode}To{entity.GetType().Name}{Separator}{entity.Id}";

        public string CreateTogglePhotoSelectionCallback(IDisplayableEntity entity, int photoId, bool select, PhotosType photosManagementMode) 
            => $"{_settings.TogglePhotoSelection}{entity.GetType().Name}{Separator}{photoId}{Separator}{select}{Separator}{photosManagementMode}{Separator}{entity.Id}";

        public string CreateMakeDefaultPhotoForEntityCallback(IDisplayableEntity entity, int photoId, PhotosType photosManagementMode)
            => $"{_settings.SetDefaultPhoto}{photosManagementMode}For{entity.GetType().Name}{Separator}{photoId}{Separator}{entity.Id}";

        public string CreateDeletePhotosForEntityCallback(IDisplayableEntity entity, PhotosType photosManagementMode)
            => $"{_settings.DeletePhoto}{photosManagementMode}For{entity.GetType().Name}{Separator}{entity.Id}";

        public string CreateDeleteMessagesCallback(int[] messagesId)
            => $"{_settings.DeleteMessages}{Separator}[{string.Join(", ", messagesId)}]";

        public string CreateDeleteMessagesCallback()
            => _settings.DeleteMessages;

        public string CreateFindColorCallback(Cat entity, string buildedColor, string colorPart)
        {
            return $"{_settings.FindColor}{entity.GetType().Name}{Separator}{(buildedColor + "_" + colorPart).TrimStart('_')}{Separator}{entity.Id}";
        }

        public string CreateStartFindColorCallback(Cat entity)
            => $"{_settings.FindColor}{entity.GetType().Name}{Separator}{entity.Id}";

        public string CreateToggleEntityVisibilityCallback(IDisplayableEntity entity)
            => $"{_settings.ToggleEntityVisibility}{entity.GetType().Name}{Separator}{entity.Id}";

        public string CreateOpenKittenClassCallback(int kittenId)
            => $"{_settings.OpenKittenClass}{Separator}{kittenId}";

        public string CreateOpenKittenStatusCallback(int kittenId)
            => $"{_settings.OpenKittenStatus}{Separator}{kittenId}";

        public string CreateSetKittenClassCallback(int kittenId, KittenClass kittenClass)
            => $"{_settings.SetKittenClass}{Separator}{kittenClass}{Separator}{kittenId}";

        public string CreateSetKittenStatusCallback(int kittenId, KittenStatus kittenStatus)
            => $"{_settings.SetKittenStatus}{Separator}{kittenStatus}{Separator}{kittenId}";

        public string GetProcessBookingCallback()
        {
            throw new NotImplementedException();
        }

        public string GetCloseBookingCallback()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
