using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;
using Microsoft.Extensions.Options;

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

        public string GetConfirmDeletePhotoCallback<T>(PhotosManagementMode mode) where T : IDisplayableEntity
            => $"{_settings.ConfirmationSuffix}{_settings.DeletePhoto}{mode}For{typeof(T).Name}";

        public string GetDeleteEntityCallback<T>() where T : IDisplayableEntity
            => $"{_settings.DeleteEntity}{typeof(T).Name}";

        public string GetDeletePhotoCallback<T>(PhotosManagementMode mode) where T : IDisplayableEntity
            => $"{_settings.DeletePhoto}{mode}For{typeof(T).Name}";

        public string GetDeleteMessagesCallback() => _settings.DeleteMessages;

        public string GetEditEntityCallback<T>() where T : IDisplayableEntity
            => $"{_settings.EditEntity}{typeof(T).Name}";

        public string GetListEntityCallback<T>() where T : IDisplayableEntity
            => $"{_settings.ListEntity}{typeof(T).Name}";

        public string GetManagePhotosCallback<T>(PhotosManagementMode mode) where T : IDisplayableEntity
            => $"{_settings.ManagePhotos}{mode}To{typeof(T).Name}";

        public string GetOpenEntityCallback<T>() where T : IDisplayableEntity
            => $"{_settings.OpenEntity}{typeof(T).Name}";

        public string GetSelectEntityCallback<T>() where T : IDisplayableEntity
            => $"{_settings.SelectEntity}{typeof(T).Name}";

        public string GetSelectToLitterCallback()
            => $"{_settings.SelectToLitter}";

        public string GetSetDefaultPhotoCallback<T>(PhotosManagementMode mode) where T : IDisplayableEntity
            => $"{_settings.SetDefaultPhoto}{mode}For{typeof(T).Name}";

        public string GetTogglePhotoSelectionCallback<T>() where T : IDisplayableEntity
            => $"{_settings.TogglePhotoSelection}{typeof(T).Name}";

        public string GetFindColorCallback<T>() where T : Cat
            => $"{_settings.FindColor}{typeof(T).Name}";

        #endregion

            #region Create methods

        public string CreateConfirmCallback(string baseCallback)
            => $"{_settings.ConfirmationSuffix}{baseCallback}";

        public string CreateEditEntityCallback(IDisplayableEntity entity)
            => $"{_settings.EditEntity}{entity.GetType().Name}{Separator}{entity.Id}";

        public string CreateSelectParentCatCallback(bool isMale, int litterId)
            => $"{_settings.SelectEntity}{nameof(ParentCat)}{Separator}{isMale}{Separator}{litterId}";

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
            string ownerCallbackData = string.Empty;
            if (unitOwner != null)
            {
                if (actionMode != ListUnitActionMode.Select)
                    throw new ArgumentException("При указанном владельце сущности режим работы может быть только Select!");
                ownerCallbackData = $"To{unitOwner.GetType().Name}{Separator}{unitOwner.Id}{Separator}";
            }
            return $"{actionMode.ToString().ToLower()}{ownerCallbackData}{entity.GetType().Name}{Separator}{entity.Id}";
        }

        public string CreateAddEntityCallback(string entityName)
            => $"{_settings.AddEntity}{entityName}";

        public string CreateManagePhotosCallback(IDisplayableEntity entity, PhotosManagementMode photosManagementMode)
            => $"{_settings.ManagePhotos}{photosManagementMode}To{entity.GetType().Name}{Separator}{entity.Id}";

        public string CreateTogglePhotoSelectionCallback(IDisplayableEntity entity, int number, bool select, PhotosManagementMode photosManagementMode) 
            => $"{_settings.TogglePhotoSelection}{entity.GetType().Name}{Separator}{number}{Separator}{select}{Separator}{photosManagementMode}{Separator}{entity.Id}";

        public string CreateMakeDefaultPhotoForEntityCallback(IDisplayableEntity entity, int photoIndex, PhotosManagementMode photosManagementMode)
            => $"{_settings.SetDefaultPhoto}{photosManagementMode}For{entity.GetType().Name}{Separator}{photoIndex}{Separator}{entity.Id}";

        public string CreateDeletePhotosForEntityCallback(IDisplayableEntity entity, PhotosManagementMode photosManagementMode)
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

        #endregion
    }
}
