using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Interfaces.Services
{
    public interface IMessagesProvider
    {
        string CreateStartMessage();

        string CreateMessagesDeletingError();

        string CreateEntityUpdateSuccessMessage();
        
        string CreateEntityUpdateFailureMessage();

        string CreateEntityNotFoundMessage();

        string CreateEntityNotFoundMessage(Type entityType, int entityId);

        string CreateEntityFormMessage(IEntity entity, bool enableEdit = true);

        string CreateEntityPhotosGuideMessage<TEntity>(TEntity entity, PhotosType photosType) where TEntity : class, IDisplayableEntity;

        string CreatePhotosLoadingMessage();

        string CreatePhotosLimitReachedMessage<TEntity>() where TEntity : class, IDisplayableEntity;

        string CreateTitlesLimitReachedMessage<TEntity>() where TEntity : class, IDisplayableEntity;

        string CreateGeneticTestsLimitReachedMessage<TEntity>() where TEntity : class, IDisplayableEntity;

        string CreateEntityPhotosMessage(IDisplayableEntity entity, int[] selectedPhotoIds, int[] photoMessageIds);

        string CreateDeleteConfirmationMessage(IDisplayableEntity entity);

        string CreateEntityDeletionSuccess();

        string CreateSelectedPhotosOverviewMessage(IDisplayableEntity entity, int photosCount);

        string CreateDeletePhotosConfirmationMessage(IDisplayableEntity entity, int[] selectedPhotoIds, int[] sendedPhotoMessageIds);

        string CreateEntityListMessage<TEntity>(ListUnitActionMode actionMode, int totalEntitiesCount, IEntity? unitOwner = null) where TEntity : class, IDisplayableEntity;

        string CreateCouldNotExtractMessagesFromCallbackMessage(CallbackQueryAdapter callback);

        string CreateParentCatSetForLitter(bool isMale, Litter litter);

        string CreateDefaultPhotoSetForEntityMessage(IDisplayableEntity entity, int photoId);

        string CreatePhotosDeletionSuccessMessage();

        string CreatePhotosDeletionFailureMessage();

        string CreateColorSetSuccessfullyMessage(string color);

        string CreateColorPickerMessage(Cat cat, string buildedColor);

        string CreateSavingSuccessMessage((int parentCatsCount, int littersCount, int kittensCount) values);

        string CreateToggleEntityVisibilitySuccessMessage(IDisplayableEntity entity);

        string CreateEntityAdditionErrorMessage();

        string CreateKittenRequiresLitterMessage();

        string CreateApiGetEntityFailureMessage();

        string CreateApiUpdateEntityFailureMessage();

        string CreateApiGetEntityAfterUpdateFailureMessage();

        string CreatePropertyUpdateFailureMessage(string propertyName);

        string CreateNoPhotosToUploadMessage();

        string CreatePhotoDownloadFailedMessage();

        string CreateApiUploadFailedMessage(int[]? unloadedPhotoIndexes = null);

        string CreateInvalidPhotoTypeSupportMessage<TEntity>(PhotosType photosType);

        string CreateApiGetPageFailureMessage<TEntity>();

        string CreateLitterNotFoundMessage(int litterId);

        string CreateLitterParentIsWrongGenderMessage(string parentName, bool isMale);

        string CreateColorUpdateErrorMessage();

        string CreateUnknownErrorMessage(string? message = null);

        string CreateEntityDeletionError();

        string CreateDefaultPhotoSetErrorMessage();

        string CreateToggleEntityVisibilityErrorMessage();

        string CreateKittenClassSelectionMenuMessage(Kitten kitten);

        string CreateKittenClassSetSuccessMessage(Kitten kitten);

        string CreateKittenStatusSelectionMenuMessage(Kitten kitten);

        string CreateKittenStatusSetSuccessMessage(Kitten kitten);

        string CreateNewBookingRequestMessage(BookingRequest bookingRequest);

        string CreateBookingProcessingMessage(BookingRequest bookingRequest, User curator);

        string CreateBookingClosedMessage();

        string CreateBookingKittenStatusChangedMessage(KittenStatus kittenStatus);

        string CreateBookingClosedWithoutKittenStatusChange();
    }
}