using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Interfaces.Markers;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types.ReplyMarkups;
using CatColor = BlueBellDolls.Common.Models.CatColor;

namespace BlueBellDolls.Bot.Providers
{
    public class KeyboardsProvider : IKeyboardsProvider
    {
        #region Fields

        private readonly CallbackDataSettings _callbackDataSettings;
        private readonly Dictionary<Type, Func<IDisplayableEntity, InlineKeyboardMarkup>> _entityOptionsKeyboards;
        private readonly ICallbackDataProvider _callbackDataProvider;
        private readonly IEnumMapperService _enumMapperService;

        #endregion

        #region Constructor

        public KeyboardsProvider(
            ICallbackDataProvider callbackDataProvider,
            IOptions<BotSettings> botOptions,
            IEnumMapperService enumMapperService)
        {
            _callbackDataProvider = callbackDataProvider;
            _callbackDataSettings = botOptions.Value.CallbackDataSettings;
            _enumMapperService = enumMapperService;

            _entityOptionsKeyboards = new Dictionary<Type, Func<IDisplayableEntity, InlineKeyboardMarkup>>
            {
                { typeof(ParentCat), entity => CreateParentCatOptions((ParentCat)entity) },
                { typeof(Litter),    entity => CreateLitterOptionsKeyboard((Litter)entity) },
                { typeof(Kitten),    entity => CreateKittenOptions((Kitten)entity) },
                { typeof(CatColor),  entity => CreateCatColorOptions((CatColor)entity) }
            };
        }

        #endregion

        #region IKeyboardsProvider Implementation

        public InlineKeyboardMarkup CreateEntityListKeyboard<TEntity>(
            IEnumerable<TEntity> entities,
            ListUnitActionMode actionMode = ListUnitActionMode.Edit,
            int rowLength = 1,
            (int page, int totalPagesCount)? pageParameters = null,
            int? litterOwnerId = null)
            where TEntity : class, IDisplayableEntity
        {
            var result = new InlineKeyboardMarkup();
            if (litterOwnerId != null)
                result.AddNewRow(CreateBackToLitterButton(litterOwnerId.Value));

            foreach (var chunk in entities.OrderBy(e => e.DisplayName).Chunk(rowLength))
                result.AddNewRow([.. chunk.Select(e => CreateListEntityReferenceButton(e, actionMode, litterOwnerId))]);

            if (pageParameters != null)
                result.AddNewRow(CreatePageControlsButtons<TEntity>(pageParameters.Value));

            if (typeof(IHandCreatableEntity).IsAssignableFrom(typeof(TEntity)) && actionMode != ListUnitActionMode.Select)
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

        public InlineKeyboardMarkup CreateEntityPhotosKeyboard(IDisplayableEntity entity, PhotosType photosType, int[] photoMessageIds, int[]? selectedPhotoIds = null)
        {
            var result = new InlineKeyboardMarkup(CreateBackToFormButton(entity, _callbackDataProvider.CreateDeleteMessagesCallback(photoMessageIds)));
            var photoIds = entity.Photos.Where(p => p.Type == photosType).Select(p => p.Id).ToArray();
            for (int i = 0; i < photoIds.Length; i++)
                result.AddNewRow(CreatePhotoReferenceButton(entity, i, photoIds[i], !selectedPhotoIds?.Contains(photoIds[i]) ?? true, photosType));

            if (selectedPhotoIds?.Length > 0)
            {
                if (selectedPhotoIds.Length == 1 && photosType == PhotosType.Photos)
                    result.AddNewRow(CreateSelectPhotoAsDefaultButton(entity, selectedPhotoIds.First(), photosType));

                result.AddNewRow(CreateDeletePhotosButton(entity, photosType));
            }

            return result;
        }

        public InlineKeyboardMarkup CreateYesNoKeyboard(string callback, IDisplayableEntity entity, string onDeletionCanceledCallback)
        {
            return new InlineKeyboardMarkup(
            [
                [
                    CreateDeleteYesButton(callback),
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

        public InlineKeyboardMarkup CreateKittenClassSelectionKeyboard(Kitten kitten)
        {
            var result = new InlineKeyboardMarkup(CreateBackToFormButton(kitten));
            foreach (var kittenClass in Enum.GetValues<KittenClass>())
            {
                if (kitten.Class == kittenClass)
                    continue;

                result.AddNewRow(
                    InlineKeyboardButton.WithCallbackData(
                        kittenClass.ToString(),
                        _callbackDataProvider.CreateSetKittenClassCallback(kitten.Id, kittenClass)
                    )
                );
            }

            return result;
        }

        public InlineKeyboardMarkup CreateKittenStatusSelectionKeyboard(Kitten kitten)
        {
            var result = new InlineKeyboardMarkup(CreateBackToFormButton(kitten));
            foreach (var kittenStatus in Enum.GetValues<KittenStatus>())
            {
                if (kitten.Status == kittenStatus)
                    continue;

                result.AddNewRow(
                    InlineKeyboardButton.WithCallbackData(
                        _enumMapperService.GetMapping(kittenStatus, kitten.IsMale),
                        _callbackDataProvider.CreateSetKittenStatusCallback(kitten.Id, kittenStatus)
                    )
                );
            }

            return result;
        }

        public ReplyKeyboardMarkup CreateStartKeyboard()
        {
            var result = new ReplyKeyboardMarkup();
            result.AddNewRow(new KeyboardButton("/litterlist"));
            result.AddNewRow(new KeyboardButton("/catlist"));
            result.AddNewRow(new KeyboardButton("/kittenlist"));
            result.AddNewRow(new KeyboardButton("/catcolorlist"));
            result.ResizeKeyboard = true;

            return result;
        }

        public InlineKeyboardMarkup CreateBookingRequestTakeCuratorshipKeyboard(BookingRequest bookingRequest)
        {
            var result = new InlineKeyboardMarkup();
            result.AddNewRow(
                InlineKeyboardButton.WithCallbackData(
                    "Взять кураторство",
                    _callbackDataProvider.CreateProcessBookingCallback(bookingRequest.Id)
                )
            );

            return result;
        }

        public InlineKeyboardMarkup CreateBookingRequestCloseKeyboard(BookingRequest bookingRequest)
        {
            var result = new InlineKeyboardMarkup();
            result.AddNewRow(
                InlineKeyboardButton.WithCallbackData(
                    "Закрыть заявку",
                    _callbackDataProvider.CreateCloseBookingCallback(bookingRequest.Id)
                )
            );
            return result;
        }

        public InlineKeyboardMarkup CreateBookingChangeKittenStatusKeyboard(int kittenId)
        {
            var result = new InlineKeyboardMarkup();
            result.AddNewRow(
                InlineKeyboardButton.WithCallbackData(
                    "Оставить без изменений",
                    _callbackDataProvider.GetClearBookingRequestKeyboardCallback()
                )
            );
            foreach(var enumValue in Enum.GetValues<KittenStatus>().Except([KittenStatus.Available]))
                result.AddNewRow(
                    InlineKeyboardButton.WithCallbackData(
                        _enumMapperService.GetMapping(enumValue),
                        _callbackDataProvider.CreateSetBookingKittenStatusCallback(kittenId, enumValue)
                    )
                );

            return result;
        }

        #endregion

        #region Private Methods

        private InlineKeyboardMarkup CreateLitterOptionsKeyboard(Litter litter)
        {
            var result = CreateEntityListKeyboard(litter.Kittens, rowLength: 3);

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

            result.AddNewRow(CreateDeleteButton(litter));

            if (litter.Photos.Count > 0)
                result.AddNewRow(CreateManagePhotosButton(litter, PhotosType.Photos));

            return result;
        }

        private InlineKeyboardMarkup CreateParentCatOptions(ParentCat parentCat)
        {
            var result = new InlineKeyboardMarkup();
            result.AddNewRow(CreateDeleteButton(parentCat));
            result.AddNewRow(CreateSelectColorButton(parentCat));

            List<InlineKeyboardButton> photosManagementRow = [];
            if (parentCat.Photos.Where(p => p.Type == PhotosType.Photos).Any())
                photosManagementRow.Add(CreateManagePhotosButton(parentCat, PhotosType.Photos));

            if (parentCat.Photos.Where(p => p.Type == PhotosType.Titles).Any())
                photosManagementRow.Add(CreateManagePhotosButton(parentCat, PhotosType.Titles));

            if (parentCat.Photos.Where(p => p.Type == PhotosType.GenTests).Any())
                photosManagementRow.Add(CreateManagePhotosButton(parentCat, PhotosType.GenTests));

            if (photosManagementRow.Count > 0)
                result.AddNewRow([.. photosManagementRow]);

            return result;
        }

        private InlineKeyboardMarkup CreateKittenOptions(Kitten kitten)
        {
            var result = new InlineKeyboardMarkup();
            result.AddNewRow(CreateBackToLitterButton(kitten.LitterId));
            result.AddNewRow(CreateDeleteButton(kitten));
            result.AddNewRow(CreateChangeKittenClassButton(kitten), CreateSelectColorButton(kitten), CreateChangeKittenStatusButton(kitten));

            if (kitten.Photos.Count > 0)
                result.AddNewRow(CreateManagePhotosButton(kitten, PhotosType.Photos));

            return result;
        }

        private InlineKeyboardMarkup CreateCatColorOptions(CatColor catColor)
        {
            var result = new InlineKeyboardMarkup();

            if (catColor.Photos.Count > 0)
                result.AddNewRow(CreateManagePhotosButton(catColor, PhotosType.Photos));

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

        private InlineKeyboardButton CreateDeleteButton(IDisplayableEntity entity, int fromLitterId = 0)
        {
            return InlineKeyboardButton.WithCallbackData(
                $"Удалить {entity.DisplayName}",
                _callbackDataProvider.CreateDeleteEntityCallback(entity, fromLitterId)
            );
        }

        private InlineKeyboardButton CreateListEntityReferenceButton(IDisplayableEntity entity, ListUnitActionMode actionMode, int? litterOwnerId = null)
        {
            return InlineKeyboardButton.WithCallbackData(
                $"{(entity.IsEnabled ? "" : "(Скрыто) ")}" + entity.DisplayName + $" (Id {entity.Id})",
                _callbackDataProvider.CreateEntityReferenceCallback(entity, actionMode, litterOwnerId)
            );
        }

        private InlineKeyboardButton CreateAddButton<TEntity>() where TEntity : class, IDisplayableEntity
            => InlineKeyboardButton.WithCallbackData(
                "Новая сущность...",
                _callbackDataProvider.CreateAddEntityCallback(typeof(TEntity).Name)
            );

        private InlineKeyboardButton CreateDeleteYesButton(string callback)
        {
            return InlineKeyboardButton.WithCallbackData("Да", $"{_callbackDataProvider.CreateConfirmCallback(callback)}");
        }

        private InlineKeyboardButton CreateDeleteNoButton(string callback)
            => InlineKeyboardButton.WithCallbackData("Нет", callback);

        private InlineKeyboardButton CreateManagePhotosButton(IDisplayableEntity entity, PhotosType photosManagementMode)
        {
            var buttonText = photosManagementMode switch
            {
                PhotosType.Photos => "Фото",
                PhotosType.Titles => "Титулы",
                PhotosType.GenTests => "Ген. тесты",
                _ => string.Empty
            };
            return InlineKeyboardButton.WithCallbackData(buttonText, _callbackDataProvider.CreateManagePhotosCallback(entity, photosManagementMode));
        }

        private InlineKeyboardButton CreatePhotoReferenceButton(IDisplayableEntity entity, int number, int photoId, bool select, PhotosType photosManagementMode)
        {
            var selectionChar = select ? "⚫️" : "🟢";

            return InlineKeyboardButton.WithCallbackData(
                $"{selectionChar}Фото {number + 1}",
                _callbackDataProvider.CreateTogglePhotoSelectionCallback(entity, photoId, select, photosManagementMode));
        }

        private InlineKeyboardButton CreateSelectPhotoAsDefaultButton(IDisplayableEntity entity, int photoId, PhotosType photosManagementMode)
            => InlineKeyboardButton.WithCallbackData(
                "Сделать основным",
                _callbackDataProvider.CreateMakeDefaultPhotoForEntityCallback(entity, photoId, photosManagementMode));

        private InlineKeyboardButton CreateDeletePhotosButton(IDisplayableEntity entity, PhotosType photosManagementMode)
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
            => InlineKeyboardButton.WithCallbackData("Окрас", _callbackDataProvider.CreateStartFindColorCallback(entity));

        private InlineKeyboardButton CreateEditEntityButton(IDisplayableEntity entity)
            => InlineKeyboardButton.WithCallbackData("Редактировать", _callbackDataProvider.CreateEditEntityCallback(entity));

        private InlineKeyboardButton CreateToggleEntityVisibilityButton(IDisplayableEntity entity)
            => InlineKeyboardButton.WithCallbackData($"{(entity.IsEnabled ? "Скрыть с сайта" : "Вернуть на сайт")}", _callbackDataProvider.CreateToggleEntityVisibilityCallback(entity));

        private InlineKeyboardButton CreateChangeKittenClassButton(Kitten kitten)
            => InlineKeyboardButton.WithCallbackData("Класс", _callbackDataProvider.CreateOpenKittenClassCallback(kitten.Id));

        private InlineKeyboardButton CreateChangeKittenStatusButton(Kitten kitten)
            => InlineKeyboardButton.WithCallbackData("Статус", _callbackDataProvider.CreateOpenKittenStatusCallback(kitten.Id));

        #endregion

    }
}