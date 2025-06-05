using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;

namespace BlueBellDolls.Bot.Providers
{
    public class MessagesProvider : IMessagesProvider
    {

        #region Fields

        private readonly EntityFormSettings _entityFormSettings;
        private readonly EntitySettings _entitySettings;
        private readonly Dictionary<Type, Func<IEntity, bool, string>> _entityFormMessages;

        #endregion

        #region Constructor

        public MessagesProvider(
            IOptions<EntityFormSettings> entityFormSettings, 
            IOptions<EntitySettings> entityOptions)
        {
            _entityFormSettings = entityFormSettings.Value;
            _entitySettings = entityOptions.Value;
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
        {
            return
                "Это главное меню панели управления сайтом BlueBellDolls.\n" +
                "\n" +
                "📌 Основные команды:\n" +
                "├ /newcat - Создать нового производителя\n" +
                "└ /newlitter - Создать новый помёт\n" +
                "\n" +
                "📂 Списки сущностей:\n" +
                "├ /catlist - Список производителей\n" +
                "├ /kittenlist - Список всех котят\n" +
                "└ /litterlist - Список всех помётов\n" +
                "\n" +
                "⚙️ Работа с данными:\n" +
                "└ /save - Отправить новые данные на сервер\n" +
                "\n" +
                "ℹ️ Инструкция по редактированию:\n" +
                "Используйте ответ на сообщение сущности с новыми значениями в формате:\n" +
                "\"Поле: Значение\"\n" +
                "\n" +
                "🔄 /start - Обновить это меню\n" +
                "══════════════════════════";
        }

        public string CreateMessagesDeletingError()
        {
            return
                "⚠️ Боту не удалось удалить сообщение с фотографиями выше.\n" +
                "Причина: ограничения Telegram (удаление возможно только в течение 48 часов)\n" +
                "Рекомендация: удалите фотографии вручную 🗑️";
        }

        public string CreateEntityUpdateSuccessMessage()
        {
            return "✅ Сущность успешно обновлена!";
        }

        public string CreateEntityUpdateFailureMessage()
        {
            return "❌ Не удалось обновить сущность! Проверьте правильность ввода данных";
        }

        public string CreateEntityNotFoundMessage()
        {
            return "🔍 Запрашиваемая сущность не найдена!";
        }

        public string CreateEntityNotFoundMessage(Type entityType, int entityId)
        {
            return $"🔍 {entityType.Name} {entityId} не найден(а)!";
        }

        public string CreateEntityFormMessage(IEntity entity, bool enableEdit = true)
        {
            return _entityFormMessages[entity.GetType()](entity, enableEdit);
        }

        public string CreateEntityPhotosGuideMessage(IDisplayableEntity entity, PhotosManagementMode photosManagementMode)
        {
            var result = photosManagementMode switch
            {
                PhotosManagementMode.Photos =>
                    $"📷 {entity.DisplayName}\n" +
                    $"├ Количество: {entity.Photos.Count}/{_entitySettings.MaxPhotosCount}\n" +
                    "└ Используйте номера для управления фото\n" +
                    "   ▪ Выберите одно как заглавное\n" +
                    "   ▪ Удалите ненужные",

                PhotosManagementMode.Titles =>
                    "🏆 Управление титулами:\n" +
                    $"├ Текущее количество: {((ParentCat)entity).Titles.Count}/{_entitySettings.MaxParentCatTitlesCount}\n" +
                    "└ Укажите номера для удаления",

                PhotosManagementMode.GenTests =>
                    "🧬 Генетические тесты:\n" +
                    $"├ Загружено: {((ParentCat)entity).GeneticTests.Count}/{_entitySettings.MaxParentCatGeneticTestsCount}\n" +
                    "└ Укажите номера для удаления",

                _ => "❌ Произошла ошибка"
            };
            return result + "\n══════════════════════════════════════════";
        }

        public string CreatePhotosLoadingMessage()
        {
            return "⏳ Загрузка...";
        }

        public string CreatePhotosLimitReachedMessage()
        {
            return $"🚫 Максимум фотографий: {_entitySettings.MaxPhotosCount}";
        }

        public string CreateTitlesLimitReachedMessage()
        {
            return $"🚫 Максимум титулов: {_entitySettings.MaxParentCatTitlesCount}";
        }

        public string CreateGeneticTestsLimitReachedMessage()
        {
            return $"🚫 Максимум тестов: {_entitySettings.MaxParentCatGeneticTestsCount}";
        }

        public string CreateEntityPhotosMessage(IDisplayableEntity entity, int[] selectedPhotoIndexes, int[] photoMessageIds)
        {
            var key = (selectedPhotoIndexes.Length > 0
                ? string.Join(", ", selectedPhotoIndexes)
                : "-") + " : " + string.Join(", ", photoMessageIds);

            return
                $"📸 {entity.DisplayName}\n" +
                "🔢 Выбранные фото (номера : ID сообщений):\n" +
                $"{key}";
        }

        public string CreateDeleteConfirmationMessage(IDisplayableEntity entity)
        {
            return
                $"⚠️ Подтверждение удаления:\n" +
                $"{entity.DisplayName} ({entity.GetType().Name} {entity.Id})\n" +
                "══════════════════════════";
        }

        public string CreateEntityDeletionSuccess()
        {
            return "✅ Сущность успешно удалена!";
        }

        public string CreateSelectedPhotosOverviewMessage(IDisplayableEntity entity, int photosCount)
        {
            var entityData = $"{entity.GetType().Name} {entity.Id}";
            return $"📷 Будет удалено {photosCount} фото у {entityData}\nПодтвердите действие";
        }

        public string CreateDeletePhotosConfirmationMessage(IDisplayableEntity entity, int[] selectedPhotoIndexes, int[] sendedPhotoMessageIds)
        {
            return
                $"⚠️ Подтвердите удаление {selectedPhotoIndexes.Length} фото:\n" +
                $"Сущность: {entity.DisplayName} ({entity.GetType().Name} {entity.Id})\n" +
                $"Номера : ID сообщений:\n" +
                $"{string.Join(", ", selectedPhotoIndexes)} : {string.Join(", ", sendedPhotoMessageIds)}";
        }

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
        {
            return $"❌ Ошибка обработки callback: {c.CallbackData}";
        }

        public string CreateParentCatSetForLitter(ParentCat parentCat, Litter litter)
        {
            var parentGender = parentCat.IsMale ? "папа" : "мама";
            return
                $"✅ Установлен родитель ({parentGender}):\n" +
                $"├ Для помёта: {litter.Letter}\n" +
                $"└ Дата рождения: {litter.BirthDay.ToString("d", new CultureInfo("ru-RU"))}";
        }

        public string CreateDefaultPhotoSetForEntityMessage(IDisplayableEntity entity, int photoIndex)
        {
            return
                $"📸 Основное фото обновлено!\n" +
                $"├ Сущность: {entity.DisplayName} ({entity.GetType().Name} {entity.Id})\n" +
                $"└ Номер фото: {photoIndex + 1}";
        }

        public string CreatePhotosDeletionSuccessMessage()
        {
            return "✅ Фотографии успешно удалены!";
        }

        public string CreatePhotosDeletionFailureMessage()
        {
            return CreateEntityNotFoundMessage();
        }

        public string CreateColorSetSuccessfullyMessage(string color)
        {
            return $"🎨 Цвет успешно установлен: {color}";
        }

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
        {
            return $"✅ Сущность \"{entity.DisplayName}\" " +
                (entity.IsEnabled 
                ? "теперь отображается на сайте!"
                : "скрыта с сайта!");
        }

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
                $"📸 Фото: {parentCat.Photos.Count}/{_entitySettings.MaxPhotosCount}\n" +
                $"🏆 Титулы: {parentCat.Titles.Count}/{_entitySettings.MaxParentCatTitlesCount}\n" +
                $"🧬 Тесты: {parentCat.GeneticTests.Count}/{_entitySettings.MaxParentCatGeneticTestsCount}\n" +
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
                $"📸 Фото: {litter.Photos.Count}/{_entitySettings.MaxPhotosCount}\n" +
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
                $"📌 Статус: {kitten.Status}\n" +
                "\n" +
                $"📝 {_entityFormSettings.KittenProperties[nameof(kitten.Description)]}:\n{kitten.Description}\n" +
                $"📸 Фото: {kitten.Photos.Count}/{_entitySettings.MaxPhotosCount}\n" +
                "══════════════════════════";
        }

        #endregion

    }
}
