using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Globalization;
using System.Text;
using Telegram.Bot.Types;
using CatColor = BlueBellDolls.Common.Models.CatColor;

namespace BlueBellDolls.Bot.Providers
{
    public class MessagesProvider : IMessagesProvider
    {

        #region Fields

        private readonly EntityFormSettings _entityFormSettings;
        private readonly Dictionary<Type, Func<IEntity, bool, string>> _entityFormMessages;
        private readonly IEnumMapperService _enumMapperService;
        private readonly ICommonMessagesProvider _commonMessagesProvider;
        private readonly IPhotosLimitsService _photosLimitsService;

        #endregion

        #region Constructor

        public MessagesProvider(
            IOptions<EntityFormSettings> entityFormSettings,
            IEnumMapperService enumMapperService,
            ICommonMessagesProvider commonMessagesProvider,
            IPhotosLimitsService photosLimitsService)
        {
            _entityFormSettings = entityFormSettings.Value;
            _enumMapperService = enumMapperService;
            _commonMessagesProvider = commonMessagesProvider;
            _photosLimitsService = photosLimitsService;
            _entityFormMessages = new()
            {
                { typeof(ParentCat), (entity, enableEdit) => CreateParentCatFormMessage((ParentCat)entity, enableEdit) },
                { typeof(Litter),    (entity, enableEdit) => CreateLitterFormMessage((Litter)entity, enableEdit) },
                { typeof(Kitten),    (entity, enableEdit) => CreateKittenFormMessage((Kitten)entity, enableEdit) },
                { typeof(CatColor),  (entity, enableEdit) => CreateCatColorFormMessage((CatColor)entity, enableEdit) }
            };
        }

        #endregion

        #region IMessagesProvider implementation

        public string CreateStartMessage()
            => "Это главное меню панели управления сайтом BlueBellDolls.\n"
             + "\n" 
             + "📌 Основные команды:\n"
             + "├ /newcat - Создать нового производителя\n"
             + "└ /newlitter - Создать новый помёт\n"
             + "\n"
             + "📂 Списки сущностей:\n"
             + "├ /catlist - Список производителей\n"
             + "├ /kittenlist - Список всех котят\n"
             + "├ /litterlist - Список всех помётов\n"
             + "└ /catcolorlist - Список всех окрасов\n"
             + "\n" 
             + "ℹ️ Инструкция по редактированию:\n" 
             + "Используйте ответ на сообщение сущности с новыми значениями в формате:\n" 
             + "\"Поле: Значение\"\n" 
             + "\n" 
             + "🔄 /start - Обновить это меню\n"
             + "══════════════════════════";

        public string CreateMessagesDeletingError()
            => "⚠️ Боту не удалось удалить сообщение с фотографиями выше.\n" + "Причина: ограничения Telegram (удаление возможно только в течение 48 часов)\n" + "Рекомендация: удалите фотографии вручную 🗑️";

        public string CreateEntityUpdateSuccessMessage()
            => "✅ Сущность успешно обновлена!";

        public string CreateEntityUpdateFailureMessage()
            => "❌ Не удалось обновить сущность! Проверьте правильность ввода данных";

        public string CreateEntityNotFoundMessage()
            => "🔍 Запрашиваемая сущность не найдена!";

        public string CreateEntityNotFoundMessage(Type entityType, int entityId)
            => $"🔍 {entityType.Name} {entityId} не найден(а)!";

        public string CreateEntityFormMessage(IEntity entity, bool enableEdit = true)
            => _entityFormMessages[entity.GetType()](entity, enableEdit);

        public string CreateEntityPhotosGuideMessage<TEntity>(TEntity entity, PhotosType photosType) where TEntity : class, IDisplayableEntity
        {
            var counter = $"{entity.Photos.Count(p => p.Type == photosType)}/{_photosLimitsService.GetLimit<TEntity>(photosType)}";
            var result = photosType switch
            {
                PhotosType.Photos =>
                    $"📷 {entity.DisplayName}\n" +
                    $"├ Количество: {counter}\n" +
                    "└ Используйте номера для управления фото\n" +
                    "   ▪ Выберите одно как заглавное\n" +
                    "   ▪ Удалите ненужные",

                PhotosType.Titles =>
                    "🏆 <b>Управление титулами</b>:\n" +
                    $"├ Текущее количество: {counter}\n" +
                    "└ Укажите номера для удаления",

                PhotosType.GenTests =>
                    "🧬 <b>Генетические тесты</b>:\n" +
                    $"├ Загружено: {counter}\n" +
                    "└ Укажите номера для удаления",

                _ => "❌ Произошла ошибка"
            };
            return result + "\n══════════════════════════════════════════";
        }

        public string CreatePhotosLoadingMessage()
            => "⏳ Загрузка...";

        public string CreatePhotosLimitReachedMessage<TEntity>() where TEntity : class, IDisplayableEntity
            => $"🚫 Максимум фотографий: {_photosLimitsService.GetLimit<TEntity>(PhotosType.Photos)}";

        public string CreateTitlesLimitReachedMessage<TEntity>() where TEntity : class, IDisplayableEntity
            => $"🚫 Максимум титулов: {_photosLimitsService.GetLimit<TEntity>(PhotosType.Titles)}";

        public string CreateGeneticTestsLimitReachedMessage<TEntity>() where TEntity : class, IDisplayableEntity
            => $"🚫 Максимум тестов: {_photosLimitsService.GetLimit<TEntity>(PhotosType.GenTests)}";

        public string CreateEntityPhotosMessage(IDisplayableEntity entity, int[] selectedPhotoIds, int[] photoMessageIds)
        {
            var key = (selectedPhotoIds.Length > 0
                ? string.Join(", ", selectedPhotoIds)
                : "-") + " : " + string.Join(", ", photoMessageIds);

            return
                $"📸 {entity.DisplayName}\n" +
                "🔢 Выбранные фото (Ключи фотографий : ID сообщений):\n" +
                $"{key}";
        }

        public string CreateDeleteConfirmationMessage(IDisplayableEntity entity)
            => $"⚠️ Подтверждение удаления:\n" + $"{entity.DisplayName} ({entity.GetType().Name} {entity.Id})\n" + "══════════════════════════";

        public string CreateEntityDeletionSuccess()
            => "✅ Сущность успешно удалена!";

        public string CreateSelectedPhotosOverviewMessage(IDisplayableEntity entity, int photosCount)
        {
            var entityData = $"{entity.GetType().Name} {entity.Id}";
            return $"📷 Будет удалено {photosCount} фото у {entityData}\nПодтвердите действие";
        }

        public string CreateDeletePhotosConfirmationMessage(IDisplayableEntity entity, int[] selectedPhotoIds, int[] sendedPhotoMessageIds)
            => $"⚠️ Подтвердите удаление <b>{selectedPhotoIds.Length}</b> фото:\n" 
             + $"Сущность: {entity.DisplayName} ({entity.GetType().Name} {entity.Id})\n" 
             + $"Ключи : ID сообщений:\n" 
             + $"{string.Join(", ", selectedPhotoIds)} : {string.Join(", ", sendedPhotoMessageIds)}";

        public string CreateEntityListMessage<TEntity>(
            ListUnitActionMode actionMode,
            int totalEntitiesCount,
            IEntity? unitOwner = null)
            where TEntity : class, IDisplayableEntity
        {
            var unitOwnerReference = unitOwner != null
                ? $" для {unitOwner.GetType()} {unitOwner.Id}"
                : string.Empty;

            return
                $"📂 {actionMode} {typeof(TEntity).Name}{unitOwnerReference}\n" +
                $"└ Количество: {totalEntitiesCount} (локально: {totalEntitiesCount})\n" +
                "══════════════════════════";
        }

        public string CreateCouldNotExtractMessagesFromCallbackMessage(CallbackQueryAdapter c)
            => $"❌ Ошибка обработки callback: {c.CallbackData}";

        public string CreateParentCatSetForLitter(bool isMale, Litter litter)
        {
            var parentGender = isMale ? "папа" : "мама";
            return
                $"✅ Установлен родитель ({parentGender}):\n" +
                $"├ Для помёта: {litter.Letter}\n" +
                $"└ Дата рождения: {litter.BirthDay.ToString("d", new CultureInfo("ru-RU"))}";
        }

        public string CreateDefaultPhotoSetForEntityMessage(IDisplayableEntity entity, int photoId)
            => $"📸 Основное фото обновлено!\n" 
             + $"├ Сущность: {entity.DisplayName} ({entity.GetType().Name} {entity.Id})\n" 
             + $"└ Номер фото: {photoId + 1}";

        public string CreatePhotosDeletionSuccessMessage()
            => "✅ Фотографии успешно удалены!";

        public string CreatePhotosDeletionFailureMessage()
            => CreateEntityNotFoundMessage();

        public string CreateColorSetSuccessfullyMessage(string color)
            => $"🎨 Окрас успешно установлен: {color}";

        public string CreateColorPickerMessage(Cat cat, string buildedColor)
        {
            var parts = buildedColor.Split('_', StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();

            sb.AppendLine($"🎨 Выбор цвета для <b>{cat.DisplayName}</b> ({cat.GetType().Name} {cat.Id})")
              .AppendLine("══════════════════════════");

            int counter = 1;
            foreach (var part in parts)
            {
                sb.AppendLine($"▫ {counter}. {part}");
                counter++;
            }

            sb.AppendLine($"▫ {counter}. ...")
              .AppendLine()
              .AppendLine($"📍 Текущий путь: {buildedColor.Replace("_", " → ")}")
              .AppendLine("─────────────────────────")
              .AppendLine("➡️ Сделайте выбор, указав нужный подпункт");

            return sb.ToString();
        }

        public string CreateSavingSuccessMessage((int parentCatsCount, int littersCount, int kittensCount) values)
        {
            if (values is {
                    parentCatsCount: 0,
                    kittensCount: 0,
                    littersCount: 0
                })
                return "🤷‍♂️В базе нету сущностей, требующих сохранения!";

            var sb = new StringBuilder($"✅ Успешно сохранены сущности ({values.parentCatsCount + values.littersCount + values.kittensCount} шт.)");
            sb
                .AppendLine()
                .AppendLine("Из них:");

            if (values.parentCatsCount > 0)
                sb.AppendLine($"- Кошки производители: {values.parentCatsCount} шт.");

            if (values.littersCount > 0)
                sb.AppendLine($"- Помёты: {values.littersCount} шт.");

            if (values.kittensCount > 0)
                sb.AppendLine($"- Котята: {values.kittensCount} шт.");

            return sb.ToString();
        }

        public string CreateToggleEntityVisibilitySuccessMessage(IDisplayableEntity entity)
        => $"✅ Сущность \"{entity.DisplayName}\" " + (entity.IsEnabled ? "теперь отображается на сайте!" : "скрыта с сайта!");

        public string CreateKittenRequiresLitterMessage() 
            => "❌ Не удалось добавить котёнка! Требуется привязка к помёту.";

        public string CreateApiGetEntityFailureMessage() 
            => "❌ Не удалось получить сущность: сервер не ответил или отклонил запрос";

        public string CreateApiUpdateEntityFailureMessage()
            => "❌ Не удалось обновить сущность: сервер не ответил или отклонил запрос";

        public string CreateApiGetEntityAfterUpdateFailureMessage()
            => "❌ Не удалось получить обновлённую сущность: сервер не ответил или отклонил запрос";

        public string CreateEntityAdditionErrorMessage()
            => "❌ Не удалось добавить сущность: сервер не ответил или отклонил запрос";

        public string CreatePropertyUpdateFailureMessage(string propertyName) 
            => $"❌ Не удалось обновить свойство \"{propertyName}\": сервер не ответил или отклонил запрос";

        public string CreateNoPhotosToUploadMessage()
            => "❌ Нет фотографий для загрузки!";

        public string CreatePhotoDownloadFailedMessage()
            => "❌ Не удалось скачать фотографию из Telegram!";

        public string CreateApiUploadFailedMessage(int[]? unloadedPhotoIndexes = null)
        {
            if (unloadedPhotoIndexes == null || unloadedPhotoIndexes.Length == 0) return "❌ Не удалось загрузить фотографии на сервер!";
            if (unloadedPhotoIndexes.Length == 1) return $"❌ Фотографию под номером {unloadedPhotoIndexes.First()} не удалось загрузить на сервер!";

            return $"❌ Фотографии под номером {string.Join(", ", unloadedPhotoIndexes)} удалось загрузить на сервер!";
        }

        public string CreateInvalidPhotoTypeSupportMessage<TEntity>(PhotosType photosType) 
            => $"❌ Сущность {typeof(TEntity).Name} не поддерживает работу с фотографиями типа {photosType}!";

        public string CreateApiGetPageFailureMessage<TEntity>()
            => $"❌ Не удалось получить страницу с сущностями {typeof(TEntity).Name}: сервер не ответил или отклонил запрос";

        public string CreateLitterNotFoundMessage(int litterId)
            => $"🔍 Помёт {litterId} не найден!";

        public string CreateLitterParentIsWrongGenderMessage(string parentName, bool isMale)
            => $"❌ Родитель {parentName} имеет неправильный пол! Ожидался представитель {(isMale ? "мужского" : "женского")} пола.";

        public string CreateColorUpdateErrorMessage()
            => "❌ Не удалось обновить цвет: сервер не ответил или отклонил запрос";

        public string CreateUnknownErrorMessage(string? message = null)
            => "❌ Произошла неизвестная ошибка" + (message != null ? $": {message}" : "");

        public string CreateEntityDeletionError()
            => "❌ Не удалось удалить сущность: сервер не ответил или отклонил запрос";

        public string CreateDefaultPhotoSetErrorMessage()
            => "❌ Не удалось установить основное фото: сервер не ответил или отклонил запрос";

        public string CreateToggleEntityVisibilityErrorMessage()
            => "❌ Не удалось изменить видимость сущности: сервер не ответил или отклонил запрос";

        public string CreateKittenClassSelectionMenuMessage(Kitten kitten)
            => $"🎖 Выбор класса для <b>{kitten.DisplayName}</b> ({kitten.GetType().Name} {kitten.Id})\n" +
               $"Текущее значение: {kitten.Class}\n" +
               "══════════════════════════\n" +
               "➡️ Сделайте выбор, указав нужный класс";

        public string CreateKittenClassSetSuccessMessage(Kitten kitten)
            => $"✅ Котёнок {kitten.DisplayName} успешно получил класс «{kitten.Class}»!";

        public string CreateKittenStatusSelectionMenuMessage(Kitten kitten)
            => $"📌 Выбор статуса для <b>{kitten.DisplayName}</b> ({kitten.GetType().Name} {kitten.Id})\n" +
               $"Текущее значение: {_enumMapperService.GetMapping(kitten.Status)}\n" +
               "══════════════════════════\n" +
               "➡️ Сделайте выбор, указав нужный статус";

        public string CreateKittenStatusSetSuccessMessage(Kitten kitten)
            => $"✅ Котёнок {kitten.DisplayName} успешно получил статус «{_enumMapperService.GetMapping(kitten.Status, kitten.IsMale)}»!";

        public string CreateNewBookingRequestMessage(BookingRequest bookingRequest)
            => _commonMessagesProvider.CreateNewBookingRequestMessage(bookingRequest);

        public string CreateBookingProcessingMessage(BookingRequest bookingRequest, User curator)
            => _commonMessagesProvider.CreateBookingRequestTemplateMessage(bookingRequest) +
            $"\n({DateTime.UtcNow.AddHours(3):t}) Назначен куратор: {curator.FirstName} (@{curator.Username})";

        public string CreateBookingClosedMessage()
            => $"\n({DateTime.UtcNow.AddHours(3):t}) Заявка закрыта. Поменяйте статус котёнка, если требуется";

        public string CreateBookingKittenStatusChangedMessage(KittenStatus kittenStatus)
            => $"\n({DateTime.UtcNow.AddHours(3):t}) Установлен статус котёнка «{_enumMapperService.GetMapping(kittenStatus)}»";

        public string CreateBookingClosedWithoutKittenStatusChange()
            => $"({DateTime.UtcNow.AddHours(3):t}) Заявка закрыта.";

        #endregion

        #region Methods

        private string CreateParentCatFormMessage(ParentCat parentCat, bool enableEdit)
        {
            return
                (enableEdit ? $"{nameof(ParentCat)} {parentCat.Id}\n\n" : "") +
                $"🐾 {_entityFormSettings.ParentCatProperties[nameof(parentCat.Name)]}: {ValueOrNull(parentCat.Name) ?? "Новый производитель"}\n" +
                $"📅 {_entityFormSettings.ParentCatProperties[nameof(parentCat.BirthDay)]}: {parentCat.BirthDay.ToString(new CultureInfo("ru-RU"))}\n" +
                $"♂♀ {_entityFormSettings.ParentCatProperties[nameof(parentCat.IsMale)]}: {(parentCat.IsMale ? "мужской" : "женский")}\n" +
                $"🎨 Окрас: {parentCat.Color?.DisplayName ?? "Не указан"}\n" +
                "\n" +
                $"📸 Фото: {parentCat.Photos.Where(p => p.Type == PhotosType.Photos).Count()}/{_photosLimitsService.GetLimit<ParentCat>(PhotosType.Photos)}\n" +
                $"🏆 Титулы: {parentCat.Photos.Where(p => p.Type == PhotosType.Titles).Count()}/{_photosLimitsService.GetLimit<ParentCat>(PhotosType.Titles)}\n" +
                $"🧬 Тесты: {parentCat.Photos.Where(p => p.Type == PhotosType.GenTests).Count()}/{_photosLimitsService.GetLimit<ParentCat>(PhotosType.GenTests)}\n" +
                "\n" +
                $"📝 {_entityFormSettings.ParentCatProperties[nameof(parentCat.Description)]}:\n{ValueOrNull(parentCat.Description) ?? "Добавьте описание!"}\n" +
                "══════════════════════════";
        }

        private string CreateLitterFormMessage(Litter litter, bool enableEdit)
        {
            return
                (enableEdit ? $"{nameof(Litter)} {litter.Id}\n\n" : "") +
                $"🔤 {_entityFormSettings.LitterProperties[nameof(litter.Letter)]}: {litter.Letter}\n" +
                $"📅 {_entityFormSettings.LitterProperties[nameof(litter.BirthDay)]}: {litter.BirthDay.ToString(new CultureInfo("ru-RU"))}\n" +
                "\n" +
                "👪 Родители:\n" +
                $"├ Мама: {litter.MotherCat?.Name ?? "—"}\n" +
                $"└ Папа: {litter.FatherCat?.Name ?? "—"}\n" +
                "\n" +
                $"📸 Фото: {litter.Photos.Count}/{_photosLimitsService.GetLimit<Litter>(PhotosType.Photos)}\n" +
                $"🐱 Котят: {litter.Kittens.Count}\n" +
                "\n" +
                $"📝 {_entityFormSettings.LitterProperties[nameof(litter.Description)]}:\n{ValueOrNull(litter.Description) ?? "Добавьте описание!"}\n" +
                "══════════════════════════";
        }

        private string CreateKittenFormMessage(Kitten kitten, bool enableEdit)
        {
            return
                (enableEdit ? $"{nameof(Kitten)} {kitten.Id}\n\n" : "") +
                $"🐾 {_entityFormSettings.KittenProperties[nameof(kitten.Name)]}: {ValueOrNull(kitten.Name) ?? "Новый котёнок"}\n" +
                $"📅 {_entityFormSettings.KittenProperties[nameof(kitten.BirthDay)]}: {kitten.BirthDay.ToString(new CultureInfo("ru-RU"))}\n" +
                $"♂♀ {_entityFormSettings.KittenProperties[nameof(kitten.IsMale)]}: {(kitten.IsMale ? "мужской" : "женский")}\n" +
                $"🎨 Окрас: {kitten.Color?.DisplayName ?? "Не указан"}\n" +
                "\n" +
                $"🏅 {_entityFormSettings.KittenProperties[nameof(kitten.Class)]}: {kitten.Class}\n" +
                $"📌 {_entityFormSettings.KittenProperties[nameof(kitten.Status)]}: {_enumMapperService.GetMapping(kitten.Status, kitten.IsMale)}\n" +
                "\n" +
                $"📝 {_entityFormSettings.KittenProperties[nameof(kitten.Description)]}:\n{ValueOrNull(kitten.Description) ?? "Добавьте описание!"}\n" +
                $"📸 Фото: {kitten.Photos.Count}/{_photosLimitsService.GetLimit<Kitten>(PhotosType.Photos)}\n" +
                "══════════════════════════";
        }

        private string CreateCatColorFormMessage(CatColor color, bool enableEdit) 
        {
            return
                (enableEdit ? $"{nameof(CatColor)} {color.Id}\n\n" : "") +
                $"🎨 Идентификатор: {ValueOrNull(color.Identifier) ?? "Неизвестен"}\n" +
                $"\n" +
                $"📝 {_entityFormSettings.CatColorProperties[nameof(color.Description)]}: {ValueOrNull(color.Description) ?? "Добавьте описание!"}\n" +
                $"📸 Фото: {color.Photos.Count}/{_photosLimitsService.GetLimit<CatColor>(PhotosType.Photos)}\n";
        }

        private static string? ValueOrNull(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value;
        }

        #endregion

    }
}
