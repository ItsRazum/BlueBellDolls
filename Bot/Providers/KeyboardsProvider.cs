using BlueBellDolls.Bot.Builders;
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
            var result = new InlineKeyboardMarkup();

            foreach (var row in _entityOptionsKeyboards[entity.GetType()](entity).InlineKeyboard)
                result.AddNewRow([.. row]);

            result.AddNewRow(CreateToggleEntityVisibilityButton(entity));
            result.AddNewRow(CreateEntityControlsButtons(entity));
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
                    new InlineKeyboardButtonBuilder()
                        .WithText("Сменить родителя")
                        .WithCallbackData(_callbackDataProvider.CreateSelectParentCatCallback(parentCat.IsMale, 1, litterId))
                        .Build()
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
                    new InlineKeyboardButtonBuilder()
                        .WithText(kittenClass.ToString())
                        .WithCallbackData(_callbackDataProvider.CreateSetKittenClassCallback(kitten.Id, kittenClass))
                        .Build()
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
                    new InlineKeyboardButtonBuilder()
                        .WithText(_enumMapperService.GetMapping(kittenStatus, kitten.IsMale))
                        .WithCallbackData(_callbackDataProvider.CreateSetKittenStatusCallback(kitten.Id, kittenStatus))
                        .Build()
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
                new InlineKeyboardButtonBuilder()
                    .WithText("Взять кураторство")
                    .WithCallbackData(_callbackDataProvider.CreateProcessBookingCallback(bookingRequest.Id))
                    .Build()
            );

            return result;
        }

        public InlineKeyboardMarkup CreateBookingRequestCloseKeyboard(BookingRequest bookingRequest)
        {
            var result = new InlineKeyboardMarkup();
            result.AddNewRow(
                new InlineKeyboardButtonBuilder()
                    .WithText("Закрыть заявку")
                    .WithCallbackData(_callbackDataProvider.CreateCloseBookingCallback(bookingRequest.Id))
                    .Build()
            );
            return result;
        }

        public InlineKeyboardMarkup CreateBookingChangeKittenStatusKeyboard(int kittenId)
        {
            var result = new InlineKeyboardMarkup();
            result.AddNewRow(
                new InlineKeyboardButtonBuilder()
                    .WithText("Оставить без изменений")
                    .WithCallbackData(_callbackDataProvider.GetClearBookingRequestKeyboardCallback())
                    .Build()
            );
            foreach (var enumValue in Enum.GetValues<KittenStatus>().Except([KittenStatus.Available]))
                result.AddNewRow(
                    new InlineKeyboardButtonBuilder()
                        .WithText(_enumMapperService.GetMapping(enumValue))
                        .WithCallbackData(_callbackDataProvider.CreateSetBookingKittenStatusCallback(kittenId, enumValue))
                        .Build()
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
                    new InlineKeyboardButtonBuilder()
                        .WithText("Добавить котёнка")
                        .WithCallbackData(_callbackDataProvider.CreateAddKittenToLitterCallback(litter.Id))
                        .Build()
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
                new InlineKeyboardButtonBuilder().WithText(motherButtonText).WithCallbackData(motherButtonCallbackData).Build(),
                new InlineKeyboardButtonBuilder().WithText(fatherButtonText).WithCallbackData(fatherButtonCallbackData).Build()
            ]);

            if (litter.Photos.Count > 0)
                result.AddNewRow(CreateManagePhotosButton(litter, PhotosType.Photos));

            return result;
        }

        private InlineKeyboardMarkup CreateParentCatOptions(ParentCat parentCat)
        {
            var result = new InlineKeyboardMarkup();

            List<InlineKeyboardButton> photosManagementRow = [];
            if (parentCat.Photos.Any(p => p.Type == PhotosType.Photos))
                photosManagementRow.Add(CreateManagePhotosButton(parentCat, PhotosType.Photos));

            if (parentCat.Photos.Any(p => p.Type == PhotosType.Titles))
                photosManagementRow.Add(CreateManagePhotosButton(parentCat, PhotosType.Titles));

            if (parentCat.Photos.Any(p => p.Type == PhotosType.GenTests))
                photosManagementRow.Add(CreateManagePhotosButton(parentCat, PhotosType.GenTests));

            if (photosManagementRow.Count > 0)
                result.AddNewRow([.. photosManagementRow]);

            return result;
        }

        private InlineKeyboardMarkup CreateKittenOptions(Kitten kitten)
        {
            var result = new InlineKeyboardMarkup();
            result.AddNewRow(CreateBackToLitterButton(kitten.LitterId));
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
        private InlineKeyboardButton[] CreatePageControlsButtons<TEntity>((int page, int totalPagesCount) pageParameters, int litterOwnerId = 0)
        {
            var entityTypeName = typeof(TEntity).Name;
            var pageCounterString = pageParameters.totalPagesCount == 0
                ? "Список пуст!"
                : $"{pageParameters.page}/{pageParameters.totalPagesCount}";

            var buttons = new List<InlineKeyboardButton?>
            {
                pageParameters.page > 1
                ? new InlineKeyboardButtonBuilder()
                    .WithText("«")
                    .WithCallbackData(_callbackDataProvider.CreateListEntityCallback(entityTypeName, pageParameters.page - 1))
                    .Build()
                : null,

                new InlineKeyboardButtonBuilder()
                    .WithText(pageCounterString)
                    .WithCallbackData("...")
                    .Build(),

                pageParameters.page < pageParameters.totalPagesCount
                ? new InlineKeyboardButtonBuilder()
                    .WithText("»")
                    .WithCallbackData(_callbackDataProvider.CreateListEntityCallback(entityTypeName, pageParameters.page + 1))
                    .Build()
                : null
            };

            return buttons.Where(b => b != null).ToArray()!;
        }

        private InlineKeyboardButton[] CreateEntityControlsButtons(IDisplayableEntity entity)
        {
            var result = new List<InlineKeyboardButton>();
            if (entity is IHandCreatableEntity)
                result.Add(CreateDeleteButton(entity));

            result.AddRange([CreateBackToEntityListButton(entity), CreateUpdateEntityFormButtton(entity)]);
            return [.. result];
        }

        private InlineKeyboardButton CreateBackToEntityListButton(IDisplayableEntity entity)
        {
            var entityName = entity.GetType().Name;
            return new InlineKeyboardButtonBuilder()
                .WithText($"📄")
                .WithCallbackData(_callbackDataProvider.CreateListEntityCallback(entityName, 1))
                .WithStyle(KeyboardButtonStyle.Success)
                .Build();
        }

        private InlineKeyboardButton CreateUpdateEntityFormButtton(IDisplayableEntity entity)
            => new InlineKeyboardButtonBuilder()
                .WithText("🔄")
                .WithCallbackData(_callbackDataProvider.CreateEditEntityCallback(entity))
                .WithStyle(KeyboardButtonStyle.Primary)
                .Build();

        private InlineKeyboardButton CreateDeleteButton(IDisplayableEntity entity, int fromLitterId = 0) 
            => new InlineKeyboardButtonBuilder()
                .WithText("🗑")
                .WithCallbackData(_callbackDataProvider.CreateDeleteEntityCallback(entity, fromLitterId))
                .WithStyle(KeyboardButtonStyle.Danger)
                .Build();

        private InlineKeyboardButton CreateBackToLitterButton(int litterId)
            => new InlineKeyboardButtonBuilder()
                .WithText("Открыть помёт")
                .WithCallbackData(_callbackDataProvider.CreateBackToLitterCallback(litterId))
                .WithStyle(KeyboardButtonStyle.Primary)
                .Build();

        private InlineKeyboardButton CreateListEntityReferenceButton(IDisplayableEntity entity, ListUnitActionMode actionMode, int? litterOwnerId = null)
            => new InlineKeyboardButtonBuilder()
                .WithText($"{(entity.IsEnabled ? "" : "(Скрыто) ")}" + entity.DisplayName + $" (Id {entity.Id})")
                .WithCallbackData(_callbackDataProvider.CreateEntityReferenceCallback(entity, actionMode, litterOwnerId))
                .Build();

        private InlineKeyboardButton CreateAddButton<TEntity>() where TEntity : class, IDisplayableEntity
            => new InlineKeyboardButtonBuilder()
                .WithText("Новая сущность...")
                .WithCallbackData(_callbackDataProvider.CreateAddEntityCallback(typeof(TEntity).Name))
                .WithStyle(KeyboardButtonStyle.Primary)
                .Build();

        private InlineKeyboardButton CreateDeleteYesButton(string callback)
            => new InlineKeyboardButtonBuilder()
                .WithText("Да")
                .WithCallbackData($"{_callbackDataProvider.CreateConfirmCallback(callback)}")
                .WithStyle(KeyboardButtonStyle.Danger)
                .Build();

        private InlineKeyboardButton CreateDeleteNoButton(string callback)
            => new InlineKeyboardButtonBuilder()
                .WithText("Нет")
                .WithCallbackData(callback)
                .WithStyle(KeyboardButtonStyle.Success)
                .Build();

        private InlineKeyboardButton CreateManagePhotosButton(IDisplayableEntity entity, PhotosType photosManagementMode)
        {
            var buttonText = photosManagementMode switch
            {
                PhotosType.Photos => "Фото",
                PhotosType.Titles => "Титулы",
                PhotosType.GenTests => "Ген. тесты",
                _ => string.Empty
            };

            return new InlineKeyboardButtonBuilder()
                .WithText(buttonText)
                .WithCallbackData(_callbackDataProvider.CreateManagePhotosCallback(entity, photosManagementMode))
                .Build();
        }

        private InlineKeyboardButton CreatePhotoReferenceButton(IDisplayableEntity entity, int number, int photoId, bool select, PhotosType photosManagementMode)
            => new InlineKeyboardButtonBuilder()
                .WithText($"{(select ? "⚫️" : "🟢")}Фото {number + 1}")
                .WithCallbackData(_callbackDataProvider.CreateTogglePhotoSelectionCallback(entity, photoId, select, photosManagementMode))
                .Build();

        private InlineKeyboardButton CreateSelectPhotoAsDefaultButton(IDisplayableEntity entity, int photoId, PhotosType photosManagementMode)
            => new InlineKeyboardButtonBuilder()
                .WithText("Сделать основным")
                .WithCallbackData(_callbackDataProvider.CreateMakeDefaultPhotoForEntityCallback(entity, photoId, photosManagementMode))
                .Build();

        private InlineKeyboardButton CreateDeletePhotosButton(IDisplayableEntity entity, PhotosType photosManagementMode)
            => new InlineKeyboardButtonBuilder()
                .WithText("Удалить выбранное")
                .WithCallbackData(_callbackDataProvider.CreateDeletePhotosForEntityCallback(entity, photosManagementMode))
                .WithStyle(KeyboardButtonStyle.Danger)
                .Build();

        private InlineKeyboardButton CreateBackToFormButton(IDisplayableEntity entity, string? callbackBeforeRedirect = null)
        {
            var callbackData = string.Empty;
            if (callbackBeforeRedirect != null)
                callbackData += callbackBeforeRedirect + _callbackDataSettings.MultipleCallbackSeparator;

            callbackData += _callbackDataProvider.CreateEditEntityCallback(entity);

            return new InlineKeyboardButtonBuilder()
                .WithText("Назад")
                .WithCallbackData(callbackData)
                .WithStyle(KeyboardButtonStyle.Primary)
                .Build();
        }

        private InlineKeyboardButton CreateColorPartReferenceButton(Cat entity, string buildedColor, string colorPart)
        {
            string buttonText = colorPart;
            if (colorPart == "/")
                buttonText = "Сохранить";

            return new InlineKeyboardButtonBuilder()
                .WithText(buttonText)
                .WithCallbackData(_callbackDataProvider.CreateFindColorCallback(entity, buildedColor, colorPart))
                .Build();
        }

        private InlineKeyboardButton CreateSelectColorButton(Cat entity)
            => new InlineKeyboardButtonBuilder()
                .WithText("Окрас")
                .WithCallbackData(_callbackDataProvider.CreateStartFindColorCallback(entity))
                .Build();

        private InlineKeyboardButton CreateEditEntityButton(IDisplayableEntity entity)
            => new InlineKeyboardButtonBuilder()
                .WithText("Редактировать")
                .WithCallbackData(_callbackDataProvider.CreateEditEntityCallback(entity))
                .Build();

        private InlineKeyboardButton CreateToggleEntityVisibilityButton(IDisplayableEntity entity)
            => new InlineKeyboardButtonBuilder()
                .WithText($"{(entity.IsEnabled ? "Скрыть с сайта" : "Вернуть на сайт")}")
                .WithCallbackData(_callbackDataProvider.CreateToggleEntityVisibilityCallback(entity))
                .Build();

        private InlineKeyboardButton CreateChangeKittenClassButton(Kitten kitten)
            => new InlineKeyboardButtonBuilder()
                .WithText("Класс")
                .WithCallbackData(_callbackDataProvider.CreateOpenKittenClassCallback(kitten.Id))
                .Build();

        private InlineKeyboardButton CreateChangeKittenStatusButton(Kitten kitten)
            => new InlineKeyboardButtonBuilder()
                .WithText("Статус")
                .WithCallbackData(_callbackDataProvider.CreateOpenKittenStatusCallback(kitten.Id))
                .Build();

        #endregion

    }
}