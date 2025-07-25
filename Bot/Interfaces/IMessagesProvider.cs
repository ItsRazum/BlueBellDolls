﻿using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;

namespace BlueBellDolls.Bot.Interfaces
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

        string CreateEntityPhotosGuideMessage(IDisplayableEntity entity, PhotosManagementMode photosManagementMode);

        string CreatePhotosLoadingMessage();

        string CreatePhotosLimitReachedMessage(IDisplayableEntity entity);

        string CreateTitlesLimitReachedMessage();

        string CreateGeneticTestsLimitReachedMessage();

        string CreateEntityPhotosMessage(IDisplayableEntity entity, int[] selectedPhotoIndexes, int[] photoMessageIds);

        string CreateDeleteConfirmationMessage(IDisplayableEntity entity);

        string CreateEntityDeletionSuccess();

        string CreateSelectedPhotosOverviewMessage(IDisplayableEntity entity, int photosCount);

        string CreateDeletePhotosConfirmationMessage(IDisplayableEntity entity, int[] selectedPhotoIndexes, int[] sendedPhotoMessageIds);

        string CreateEntityListMessage<TEntity>(ListUnitActionMode actionMode, int totalEntitiesCount, IEntity? unitOwner = null) where TEntity : class, IDisplayableEntity;

        string CreateCouldNotExtractMessagesFromCallbackMessage(CallbackQueryAdapter callback);

        string CreateParentCatSetForLitter(ParentCat parentCat, Litter litter);

        string CreateDefaultPhotoSetForEntityMessage(IDisplayableEntity entity, int photoIndex);

        string CreatePhotosDeletionSuccessMessage();

        string CreatePhotosDeletionFailureMessage();

        string CreateColorSetSuccessfullyMessage(string color);

        string CreateColorPickerMessage(Cat cat, string buildedColor);

        string CreateSavingSuccessMessage((int parentCatsCount, int littersCount, int kittensCount) values);

        string CreateToggleEntityVisibilitySuccessMessage(IDisplayableEntity entity);
    }
}