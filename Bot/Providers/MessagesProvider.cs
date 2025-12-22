using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Settings;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;
using BlueBellDolls.Common.Types;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Bot.Interfaces.Services;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Providers
{
    public class MessagesProvider : IMessagesProvider
    {

        #region Fields

        private readonly EntityFormSettings _entityFormSettings;
        private readonly EntitySettings _entitySettings;
        private readonly Dictionary<Type, Func<IEntity, bool, string>> _entityFormMessages;
        private readonly IEnumMapperService _enumMapperService;
        private readonly ICommonMessagesProvider _commonMessagesProvider;

        #endregion

        #region Constructor

        public MessagesProvider(
            IOptions<EntityFormSettings> entityFormSettings, 
            IOptions<EntitySettings> entityOptions,
            IEnumMapperService enumMapperService,
            ICommonMessagesProvider commonMessagesProvider)
        {
            _entityFormSettings = entityFormSettings.Value;
            _entitySettings = entityOptions.Value;
            _enumMapperService = enumMapperService;
            _commonMessagesProvider = commonMessagesProvider;
            _entityFormMessages = new()
            {
                { typeof(ParentCat), (entity, enableEdit) => CreateParentCatFormMessage((ParentCat)entity, enableEdit) },
                { typeof(Litter),    (entity, enableEdit) => CreateLitterFormMessage((Litter)entity, enableEdit) },
                { typeof(Kitten),    (entity, enableEdit) => CreateKittenFormMessage((Kitten)entity, enableEdit) }
            };
        }

        #endregion

        #region IMessagesProvider implementation

        public string CreateStartMessage()
            => "Это главное меню панели управления сайтом BlueBellDolls.\n" 
             + "\n" + "📌 Основные команды:\n" 
             + "├ /newcat - Создать нового производителя\n" 
             + "└ /newlitter - Создать новый помёт\n" 
             + "\n" 
             + "📂 Списки сущностей:\n" 
             + "├ /catlist - Список производителей\n" 
             + "├ /kittenlist - Список всех котят\n" 
             + "└ /litterlist - Список всех помётов\n" 
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

        public string CreateEntityPhotosGuideMessage(IDisplayableEntity entity, PhotosType photosManagementMode)
        {
            var result = photosManagementMode switch
            {
                PhotosType.Photos =>
                    $"📷 {entity.DisplayName}\n" +
                    $"├ Количество: {entity.Photos.Where(p => p.Type == PhotosType.Photos).Count()}/{_entitySettings.MaxPhotos[entity.GetType().Name]}\n" +
                    "└ Используйте номера для управления фото\n" +
                    "   ▪ Выберите одно как заглавное\n" +
                    "   ▪ Удалите ненужные",

                PhotosType.Titles =>
                    "🏆 Управление титулами:\n" +
                    $"├ Текущее количество: {((ParentCat)entity).Photos.Where(p => p.Type == PhotosType.Titles).Count()}/{_entitySettings.MaxParentCatTitlesCount}\n" +
                    "└ Укажите номера для удаления",

                PhotosType.GenTests =>
                    "🧬 Генетические тесты:\n" +
                    $"├ Загружено: {((ParentCat)entity).Photos.Where(p => p.Type == PhotosType.GenTests).Count()}/{_entitySettings.MaxParentCatGeneticTestsCount}\n" +
                    "└ Укажите номера для удаления",

                _ => "❌ Произошла ошибка"
            };
            return result + "\n══════════════════════════════════════════";
        }

        public string CreatePhotosLoadingMessage()
            => "⏳ Загрузка...";

        public string CreatePhotosLimitReachedMessage(IDisplayableEntity entity)
            => $"🚫 Максимум фотографий: {_entitySettings.MaxPhotos[entity.GetType().Name]}";

        public string CreateTitlesLimitReachedMessage()
            => $"🚫 Максимум титулов: {_entitySettings.MaxParentCatTitlesCount}";

        public string CreateGeneticTestsLimitReachedMessage()
            => $"🚫 Максимум тестов: {_entitySettings.MaxParentCatGeneticTestsCount}";

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
            => $"⚠️ Подтвердите удаление {selectedPhotoIds.Length} фото:\n" 
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

        public string CreateParentCatSetForLitter(ParentCat parentCat, Litter litter)
        {
            var parentGender = parentCat.IsMale ? "папа" : "мама";
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
            => $"🎨 Цвет успешно установлен: {color}";

        public string CreateColorPickerMessage(Cat cat, string buildedColor)
        {
            var parts = buildedColor.Split('_', StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();

            sb.AppendLine($"🎨 Выбор цвета для {cat.DisplayName} ({cat.GetType().Name} {cat.Id})")
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
            => $"🎖 Выбор класса для {kitten.DisplayName} ({kitten.GetType().Name} {kitten.Id})\n" +
               $"Текущее значение: {kitten.Class}\n" +
               "══════════════════════════\n" +
               "➡️ Сделайте выбор, указав нужный класс";

        public string CreateKittenClassSetSuccessMessage(Kitten kitten)
            => $"✅ Котёнок {kitten.DisplayName} успешно получил класс «{kitten.Class}»!";

        public string CreateKittenStatusSelectionMenuMessage(Kitten kitten)
            => $"📌 Выбор статуса для {kitten.DisplayName} ({kitten.GetType().Name} {kitten.Id})\n" +
               $"Текущее значение: {_enumMapperService.GetMapping(kitten.Status)}\n" +
               "══════════════════════════\n" +
               "➡️ Сделайте выбор, указав нужный статус";

        public string CreateKittenStatusSetSuccessMessage(Kitten kitten)
            => $"✅ Котёнок {kitten.DisplayName} успешно получил статус «{_enumMapperService.GetMapping(kitten.Status, kitten.IsMale)}»!";

        public string CreateNewBookingRequestMessage(BookingRequest bookingRequest)
            => _commonMessagesProvider.CreateNewBookingRequestMessage(bookingRequest);

        public string CreateBookingProcessingMessage(BookingRequest bookingRequest, User curator)
            => _commonMessagesProvider.CreateBookingRequestTemplateMessage(bookingRequest) +
            $"({DateTime.UtcNow.AddHours(3):t}) Назначен куратор: {curator.FirstName} (@{curator.Username})";

        public string CreateBookingCloseMessage(BookingRequest bookingRequest, User curator)
            => _commonMessagesProvider.CreateBookingRequestTemplateMessage(bookingRequest) +
            $"({DateTime.UtcNow.AddHours(3):t}) Заявка обработана (Куратор {curator.FirstName} (@{curator.Username}))";

        #endregion

        #region Methods

        private string CreateParentCatFormMessage(ParentCat parentCat, bool enableEdit)
        {
            return
                (enableEdit ? $"{nameof(ParentCat)} {parentCat.Id}\n\n" : "") +
                $"🐾 {_entityFormSettings.ParentCatProperties[nameof(parentCat.Name)]}: {parentCat.Name}\n" +
                $"📅 {_entityFormSettings.ParentCatProperties[nameof(parentCat.BirthDay)]}: {parentCat.BirthDay.ToString(new CultureInfo("ru-RU"))}\n" +
                $"♂♀ {_entityFormSettings.ParentCatProperties[nameof(parentCat.IsMale)]}: {(parentCat.IsMale ? "мужской" : "женский")}\n" +
                $"🎨 Окрас: {parentCat.Color}\n" +
                "\n" +
                $"📸 Фото: {parentCat.Photos.Where(p => p.Type == PhotosType.Photos).Count()}/{_entitySettings.MaxPhotos[nameof(ParentCat)]}\n" +
                $"🏆 Титулы: {parentCat.Photos.Where(p => p.Type == PhotosType.Titles).Count()}/{_entitySettings.MaxParentCatTitlesCount}\n" +
                $"🧬 Тесты: {parentCat.Photos.Where(p => p.Type == PhotosType.GenTests).Count()}/{_entitySettings.MaxParentCatGeneticTestsCount}\n" +
                "\n" +
                $"📝 {_entityFormSettings.ParentCatProperties[nameof(parentCat.Description)]}:\n{parentCat.Description}\n" +
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
                $"📸 Фото: {litter.Photos.Count}/{_entitySettings.MaxPhotos[nameof(Litter)]}\n" +
                $"🐱 Котят: {litter.Kittens.Count}\n" +
                "\n" +
                $"📝 {_entityFormSettings.LitterProperties[nameof(litter.Description)]}:\n{litter.Description}\n" +
                "══════════════════════════";
        }

        private string CreateKittenFormMessage(Kitten kitten, bool enableEdit)
        {
            return
                (enableEdit ? $"{nameof(Kitten)} {kitten.Id}\n\n" : "") +
                $"🐾 {_entityFormSettings.KittenProperties[nameof(kitten.Name)]}: {kitten.Name}\n" +
                $"📅 {_entityFormSettings.KittenProperties[nameof(kitten.BirthDay)]}: {kitten.BirthDay.ToString(new CultureInfo("ru-RU"))}\n" +
                $"♂♀ {_entityFormSettings.KittenProperties[nameof(kitten.IsMale)]}: {(kitten.IsMale ? "мужской" : "женский")}\n" +
                $"🎨 Окрас: {kitten.Color}\n" +
                "\n" +
                $"🏅 Класс: {kitten.Class}\n" +
                $"📌 Статус: {_enumMapperService.GetMapping(kitten.Status)}\n" +
                "\n" +
                $"📝 {_entityFormSettings.KittenProperties[nameof(kitten.Description)]}:\n{kitten.Description}\n" +
                $"📸 Фото: {kitten.Photos.Count}/{_entitySettings.MaxPhotos[nameof(Kitten)]}\n" +
                "══════════════════════════";
        }

        #endregion

    }
}
