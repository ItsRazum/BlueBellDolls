using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types.ReplyMarkups;
using static Grpc.Core.Metadata;

namespace BlueBellDolls.Bot.Providers
{
    public class KeyboardsProvider : IKeyboardsProvider
    {
        #region Fields

        private readonly CallbackDataSettings _callbackDataSettings;
        private readonly Dictionary<Type, Func<IDisplayableEntity, InlineKeyboardMarkup>> _entityOptionsKeyboards;
        private readonly ICallbackDataProvider _callbackDataProvider;

        #endregion

        #region Constructor

        public KeyboardsProvider(
            ICallbackDataProvider callbackDataProvider,
            IOptions<BotSettings> botOptions)
        {
            _callbackDataProvider = callbackDataProvider;
            _callbackDataSettings = botOptions.Value.CallbackDataSettings;

            _entityOptionsKeyboards = new Dictionary<Type, Func<IDisplayableEntity, InlineKeyboardMarkup>>
            {
                { typeof(ParentCat), entity => CreateParentCatOptions((ParentCat)entity) },
                { typeof(Litter),    entity => CreateLitterOptionsKeyboard((Litter)entity) },
                { typeof(Kitten),    entity => CreateKittenOptions((Kitten)entity) },
            };
        }

        #endregion

        #region IKeyboardsProvider Implementation

        public InlineKeyboardMarkup CreateEntityListKeyboard<TEntity>(
            IEnumerable<TEntity> entities,
            ListUnitActionMode actionMode = ListUnitActionMode.Edit,
            int chunkSize = 1,
            (int page, int totalPagesCount)? pageParameters = null,
            IEntity? unitOwner = null)
            where TEntity : class, IDisplayableEntity
        {
            var result = new InlineKeyboardMarkup();
            if (unitOwner != null)
                result.AddNewRow(CreateBackToLitterButton(unitOwner.Id));

            foreach (var chunk in entities.OrderBy(e => e.DisplayName).Chunk(chunkSize))
                result.AddNewRow([.. chunk.Select(e => CreateListEntityReferenceButton(e, actionMode, unitOwner))]);

            if (pageParameters != null)
                result.AddNewRow(CreatePageControlsButtons<TEntity>(pageParameters.Value));

            if (typeof(TEntity) != typeof(Kitten) && actionMode != ListUnitActionMode.Select)
                result.AddNewRow(CreateAddButton<TEntity>());

            return result;
        }

        public InlineKeyboardMarkup CreateEntityOptionsKeyboard(IDisplayableEntity entity)
        {
            var result = new InlineKeyboardMarkup(CreateBackToEntityListButton(entity));

            foreach (var row in _entityOptionsKeyboards[entity.GetType()](entity).InlineKeyboard)
                result.AddNewRow([.. row]);

            result.AddNewRow(CreateToggleEntityVisibilityButton(entity));
            return result;
        }

        public InlineKeyboardMarkup CreateEntityPhotosKeyboard(IDisplayableEntity entity, PhotosManagementMode photosManagementMode, int[] photoMessageIds, int[]? selectedPhotosIndexes = null)
        {
            var result = new InlineKeyboardMarkup(CreateBackToFormButton(entity, _callbackDataProvider.CreateDeleteMessagesCallback(photoMessageIds)));
            var photoIndexes = photosManagementMode switch
            {
                PhotosManagementMode.Photos => entity.Photos.Count,
                PhotosManagementMode.Titles => ((ParentCat)entity).Titles.Count,
                PhotosManagementMode.GenTests => ((ParentCat)entity).GeneticTests.Count,
                _ => 0
            };
            for (int i = 0; i < photoIndexes; i++)
                result.AddNewRow(CreatePhotoReferenceButton(entity, i, !selectedPhotosIndexes?.Contains(i) ?? true, photosManagementMode));

            if (selectedPhotosIndexes?.Length > 0)
            {
                if (selectedPhotosIndexes.Length == 1 && photosManagementMode == PhotosManagementMode.Photos)
                    result.AddNewRow(CreateSelectPhotoAsDefaultButton(entity, selectedPhotosIndexes.First(), photosManagementMode));

                result.AddNewRow(CreateDeletePhotosButton(entity, photosManagementMode));
            }

            return result;
        }

        public InlineKeyboardMarkup CreateYesNoKeyboard(string callback, IDisplayableEntity entity, string onDeletionCanceledCallback, params string[] callbacksAfterDeletion)
        {
            return new InlineKeyboardMarkup(
            [
                [
                    CreateDeleteYesButton(callback, callbacksAfterDeletion),
                    CreateDeleteNoButton(onDeletionCanceledCallback)
                ]
            ]);
        }

        public InlineKeyboardMarkup CreateEntityFromLitterKeyboard(IDisplayableEntity entity, int litterId)
        {
            var result = new InlineKeyboardMarkup(CreateBackToLitterButton(litterId));

            if (entity is ParentCat parentCat)
                result.AddNewRow(
                    InlineKeyboardButton.WithCallbackData(
                        "Сменить родителя",
                        _callbackDataProvider.CreateSelectParentCatCallback(parentCat.IsMale, 1, litterId)
                        )
                    );

            result.AddNewRow(CreateEditEntityButton(entity));

            return result;
        }

        public InlineKeyboardMarkup CreateColorPickerKeyboard(Cat entity, string buildedColor, string[] findedColorParts)
        {
            var result = new InlineKeyboardMarkup(CreateBackToFormButton(entity));

            foreach (var colorPart in findedColorParts)
                result.AddNewRow(CreateColorPartReferenceButton(entity, buildedColor, colorPart));

            return result;
        }

        public ReplyKeyboardMarkup CreateStartKeyboard()
        {
            var result = new ReplyKeyboardMarkup();
            result.AddNewRow(new KeyboardButton("/litterlist"));
            result.AddNewRow(new KeyboardButton("/catlist"));
            result.AddNewRow(new KeyboardButton("/kittenlist"));
            result.ResizeKeyboard = true;

            return result;
        }

        #endregion

        #region Private Methods

        private InlineKeyboardMarkup CreateLitterOptionsKeyboard(Litter litter)
        {
            var result = CreateEntityListKeyboard(litter.Kittens, chunkSize: 3);

            if (litter.Kittens.Count < 10)
                result.AddNewRow(
                    InlineKeyboardButton.WithCallbackData(
                        "Добавить котёнка",
                        _callbackDataProvider.CreateAddKittenToLitterCallback(litter.Id)
                    )
                );

            (string motherButtonText, string motherButtonCallbackData) = (
                "Мама",
                litter.MotherCat != null
                    ? _callbackDataProvider.CreateOpenEntityInLitterCallback(litter.MotherCat, litter.Id)
                    : _callbackDataProvider.CreateSelectParentCatCallback(false, 1, litter.Id)
            );

            (string fatherButtonText, string fatherButtonCallbackData) = (
                "Папа",
                litter.FatherCat != null
                    ? _callbackDataProvider.CreateOpenEntityInLitterCallback(litter.FatherCat, litter.Id)
                    : _callbackDataProvider.CreateSelectParentCatCallback(true, 1, litter.Id)
            );

            result.AddNewRow(
            [
                InlineKeyboardButton.WithCallbackData(motherButtonText, motherButtonCallbackData),
                InlineKeyboardButton.WithCallbackData(fatherButtonText, fatherButtonCallbackData)
            ]);

            result.AddNewRow(CreateDeleteButton(litter, _callbackDataProvider.CreateDeleteEntityCallback(litter)));

            if (litter.Photos.Count > 0)
                result.AddNewRow(CreateManagePhotosButton(litter, PhotosManagementMode.Photos));

            return result;
        }

        private InlineKeyboardMarkup CreateParentCatOptions(ParentCat parentCat)
        {
            var result = new InlineKeyboardMarkup();
            result.AddNewRow(CreateDeleteButton(parentCat, _callbackDataProvider.CreateDeleteEntityCallback(parentCat)));
            result.AddNewRow(CreateSelectColorButton(parentCat));

            List<InlineKeyboardButton> photosManagementRow = [];
            if (parentCat.Photos.Count > 0)
                photosManagementRow.Add(CreateManagePhotosButton(parentCat, PhotosManagementMode.Photos));

            if (parentCat.Titles.Count > 0)
                photosManagementRow.Add(CreateManagePhotosButton(parentCat, PhotosManagementMode.Titles));

            if (parentCat.GeneticTests.Count > 0)
                photosManagementRow.Add(CreateManagePhotosButton(parentCat, PhotosManagementMode.GenTests));

            if (photosManagementRow.Count > 0)
                result.AddNewRow([.. photosManagementRow]);

            return result;
        }

        private InlineKeyboardMarkup CreateKittenOptions(Kitten kitten)
        {
            var result = new InlineKeyboardMarkup();
            result.AddNewRow(CreateBackToLitterButton(kitten.LitterId));
            result.AddNewRow(CreateDeleteButton(kitten, _callbackDataProvider.CreateDeleteEntityCallback(kitten, kitten.LitterId)));
            result.AddNewRow(CreateSelectColorButton(kitten));

            if (kitten.Photos.Count > 0)
                result.AddNewRow(CreateManagePhotosButton(kitten, PhotosManagementMode.Photos));

            return result;
        }

        //TODO: Исправить смену страниц при выборе ParentCat в помёт
        private InlineKeyboardButton[] CreatePageControlsButtons<TEntity>((int page, int totalPagesCount) pageParameters)
        {
            var entityTypeName = typeof(TEntity).Name;
            var pageCounterString = pageParameters.totalPagesCount == 0
                ? "Список пуст!"
                : $"{pageParameters.page}/{pageParameters.totalPagesCount}";
            var buttons = new List<InlineKeyboardButton?>
        {
            pageParameters.page > 1
                ? InlineKeyboardButton.WithCallbackData("«", _callbackDataProvider.CreateListEntityCallback(entityTypeName, pageParameters.page - 1))
                : null,
            InlineKeyboardButton.WithCallbackData(pageCounterString, "..."),
            pageParameters.page < pageParameters.totalPagesCount
                ? InlineKeyboardButton.WithCallbackData("»", _callbackDataProvider.CreateListEntityCallback(entityTypeName, pageParameters.page + 1))
                : null
        };

            return buttons.Where(b => b != null).ToArray()!;
        }

        private InlineKeyboardButton CreateBackToEntityListButton(IDisplayableEntity entity)
        {
            var entityName = entity.GetType().Name;
            return InlineKeyboardButton.WithCallbackData(
                $"Открыть лист {entityName}",
                _callbackDataProvider.CreateListEntityCallback(entityName, 1)
            );
        }

        private InlineKeyboardButton CreateBackToLitterButton(int litterId)
            => InlineKeyboardButton.WithCallbackData(
                "Открыть помёт",
                _callbackDataProvider.CreateBackToLitterCallback(litterId)
            );

        private InlineKeyboardButton CreateDeleteButton(IDisplayableEntity entity, string deleteCallback)
        {
            return InlineKeyboardButton.WithCallbackData(
                    $"Удалить {entity.DisplayName}",
                    deleteCallback
                );
        }

        private InlineKeyboardButton CreateListEntityReferenceButton(IDisplayableEntity entity, ListUnitActionMode actionMode, IEntity? unitOwner = null)
        {
            return InlineKeyboardButton.WithCallbackData(
                $"{(entity.IsEnabled ? "" : "(Скрыто) ")}" + entity.DisplayName + $" (Id {entity.Id})",
                _callbackDataProvider.CreateEntityReferenceCallback(entity, actionMode, unitOwner)
            );
        }

        private InlineKeyboardButton CreateAddButton<TEntity>() where TEntity : class, IDisplayableEntity
            => InlineKeyboardButton.WithCallbackData(
                "Новая сущность...",
                _callbackDataProvider.CreateAddEntityCallback(typeof(TEntity).Name)
            );

        private InlineKeyboardButton CreateDeleteYesButton(string callback, params string[] callbacksAfterDeletion)
        {
            string[] callbacks = [_callbackDataProvider.CreateConfirmCallback(callback), .. callbacksAfterDeletion];
            return InlineKeyboardButton.WithCallbackData("Да", $"{string.Join(_callbackDataSettings.MultipleCallbackSeparator, callbacks)}");
        }

        private InlineKeyboardButton CreateDeleteNoButton(string callback)
            => InlineKeyboardButton.WithCallbackData("Нет", callback);

        private InlineKeyboardButton CreateManagePhotosButton(IDisplayableEntity entity, PhotosManagementMode photosManagementMode)
        {
            var buttonText = photosManagementMode switch
            {
                PhotosManagementMode.Photos => "Фото",
                PhotosManagementMode.Titles => "Титулы",
                PhotosManagementMode.GenTests => "Ген. тесты",
                _ => string.Empty
            };
            return InlineKeyboardButton.WithCallbackData(buttonText, _callbackDataProvider.CreateManagePhotosCallback(entity, photosManagementMode));
        }
        private InlineKeyboardButton CreatePhotoReferenceButton(IDisplayableEntity entity, int number, bool select, PhotosManagementMode photosManagementMode)
        {
            var selectionChar = select ? "⚫️" : "🟢";

            return InlineKeyboardButton.WithCallbackData(
                $"{selectionChar}Фото {number + 1}",
                _callbackDataProvider.CreateTogglePhotoSelectionCallback(entity, number, select, photosManagementMode));
        }

        private InlineKeyboardButton CreateSelectPhotoAsDefaultButton(IDisplayableEntity entity, int photoIndex, PhotosManagementMode photosManagementMode)
            => InlineKeyboardButton.WithCallbackData(
                "Сделать основным",
                _callbackDataProvider.CreateMakeDefaultPhotoForEntityCallback(entity, photoIndex, photosManagementMode));

        private InlineKeyboardButton CreateDeletePhotosButton(IDisplayableEntity entity, PhotosManagementMode photosManagementMode)
            => InlineKeyboardButton.WithCallbackData(
                "Удалить выбранное",
                _callbackDataProvider.CreateDeletePhotosForEntityCallback(entity, photosManagementMode));

        private InlineKeyboardButton CreateBackToFormButton(IDisplayableEntity entity, string? callbackBeforeRedirect = null)
        {
            var callbackData = string.Empty;
            if (callbackBeforeRedirect != null)
                callbackData += callbackBeforeRedirect + _callbackDataSettings.MultipleCallbackSeparator;

            callbackData += _callbackDataProvider.CreateEditEntityCallback(entity);

            return InlineKeyboardButton.WithCallbackData("Назад", callbackData);
        }

        private InlineKeyboardButton CreateColorPartReferenceButton(Cat entity, string buildedColor, string colorPart)
        {
            string buttonText = colorPart;
            if (colorPart == "/")
                buttonText = "Сохранить";

            return InlineKeyboardButton.WithCallbackData(buttonText, _callbackDataProvider.CreateFindColorCallback(entity, buildedColor, colorPart));
        }

        private InlineKeyboardButton CreateSelectColorButton(Cat entity)
            => InlineKeyboardButton.WithCallbackData("Указать окрас", _callbackDataProvider.CreateStartFindColorCallback(entity));

        private InlineKeyboardButton CreateEditEntityButton(IDisplayableEntity entity)
            => InlineKeyboardButton.WithCallbackData("Редактировать", _callbackDataProvider.CreateEditEntityCallback(entity));

        private InlineKeyboardButton CreateToggleEntityVisibilityButton(IDisplayableEntity entity)
            => InlineKeyboardButton.WithCallbackData($"{(entity.IsEnabled ? "Скрыть с сайта" : "Вернуть на сайт")}", _callbackDataProvider.CreateToggleEntityVisibilityCallback(entity));

        #endregion

    }
}