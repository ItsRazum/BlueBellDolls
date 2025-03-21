using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace BlueBellDolls.Bot.Providers
{
    public class MessagesProvider : IMessagesProvider
    {

        #region Fields

        private readonly EntityFormSettings _entityFormSettings;
        private readonly Dictionary<Type, Func<IEntity, string>> _entityFormMessages;

        #endregion

        #region Constructor

        public MessagesProvider(IOptions<EntityFormSettings> entityFormSettings)
        {
            _entityFormSettings = entityFormSettings.Value;
            _entityFormMessages = new()
            {
                { typeof(ParentCat), (entity) => CreateParentCatFormMessage((ParentCat)entity) },
                { typeof(Litter),    (entity) => CreateLitterFormMessage((Litter)entity) },
                { typeof(Kitten),    (entity) => CreateKittenFormMessage((Kitten)entity) }
            };
        }

        #endregion

        #region IMessagesProvider implementation

        public string CreateStartMessage()
        {
            return
                "Это главное меню панели управления сайтом BlueBellDolls.\n" +
                "\n" +
                "- /newcat - Создать нового производителя.\n" +
                "- /newlitter - Создать новый помёт.\n" +
                "\n" +
                "- /catlist - Список производителей. Через него можно редактировать их данные.\n" +
                "- /kittenlist - Список всех котят. Через него можно редактировать их данные.\n" +
                "- /litterlist - Список всех помётов. Через него можно редактировать их данные, включая каждого котёнка индивидуально.\n" +
                "\n" +
                "Работа с данными:\n" +
                "- /save - отправить новые данные на сервер. После сохранения новая информация начнёт отображаться на сайте.\n" +
                "- /delete [Kitten/Cat/Litter] [id] - Удалить сущность по Id. Его легко найти, он всегда будет отображаться вместе со всей информацией о редактируемой или просматриваемой сущности\n" +
                "Для изменения любой сущности необходимо открыть его информацию (через команды list, которые были описаны блоком выше), и ответом на сообщение написать пункт и значение для него. Например если вы хотите изменить имя - выбираете реплаем и пишете \"Имя: BigBoss\". Можно вводить по несколько пунктов одновременно, однако нужно переносить строку, чтобы получился формат анкеты.\n" +
                "\n" +
                "- /start - вызов данного меню";
        }

        public string CreateMessagesDeletingError()
        {
            return
                "Боту не удалось удалить сообщение с фотографиями выше. " +
                "Это случилось из-за ограничений Telegram, согласно которым бот может удалить отправленное собой сообщение в течение 48 часов. " +
                "Ради вашего удобства рекомендуется удалить фотографии выше вручную.";
        }
        public string CreateEntityFormMessage(IEntity entity)
        {
            return _entityFormMessages[entity.GetType()](entity);
        }

        public string CreateEntityPhotosGuideMessage(IDisplayableEntity entity)
        {
            return 
                $"{entity.GetType().Name} {entity.Id}\n" +
                $"\n" +
                $"Количество фотографий: {entity.Photos.Count}/5\n" +
                $"\n" +
                $"Нумерация фотографий соответствует порядку, отправленному выше. Можно выбрать одно фото и установить его в качестве заглавного для сущности, оно может быть только одно. Также можно выбрать одно или несколько фотографий и удалить их";
        }

        public string CreateEntityPhotosMessage(IDisplayableEntity entity, int[] selectedPhotoIndexes, int[] photoMessageIds)
        {
            var key = (selectedPhotoIndexes.Length > 0
                ? string.Join(", ", selectedPhotoIndexes)
                : "-") + " : " + string.Join(", ", photoMessageIds);

            return
                $"{entity.GetType().Name} {entity.Id}\n" +
                $"Выберите фотографии\n" +
                $"\n" +
                $"{key}";
        }


        public string CreateDeleteConfirmationMessage(IDisplayableEntity entity)
        {
            return $"Вы уверены, что хотите удалить {entity.DisplayName} ({entity.GetType().Name} {entity.Id})?";
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

            return $"{actionMode} {typeof(TEntity).Name}{unitOwnerReference}\n" +
                $"\n" +
                $"Количество: {totalEntitiesCount}\n" +
                $"Хранится локально: {totalEntitiesCount}\n";
        }

        #endregion

        #region Methods

        private string CreateParentCatFormMessage(ParentCat parentCat)
        {
            return
                $"{nameof(ParentCat)} {parentCat.Id}\n" +
                $"\n" +
                $"{_entityFormSettings.ParentCatProperties[nameof(parentCat.Name)]}: {parentCat.Name}\n" +
                $"{_entityFormSettings.ParentCatProperties[nameof(parentCat.BirthDay)]}: {parentCat.BirthDay.ToString(new CultureInfo("ru-RU"))}\n" +
                $"{_entityFormSettings.ParentCatProperties[nameof(parentCat.IsMale)]}: {(parentCat.IsMale ? "мужской" : "женский")}\n" +
                $"{_entityFormSettings.ParentCatProperties[nameof(parentCat.Description)]}: {parentCat.Description}\n" +
                $"Фотографии: {parentCat.Photos.Count}/5\n" +
                $"Титулы: {parentCat.Titles.Count}/6\n" +
                $"Генетический тест 1: {(!string.IsNullOrWhiteSpace(parentCat.GeneticTestOne) ? "получен" : "нужно отправить")}\n" +
                $"Генетический тест 2: {(!string.IsNullOrWhiteSpace(parentCat.GeneticTestTwo) ? "получен" : "нужно отправить")}";
        }

        private string CreateLitterFormMessage(Litter litter)
        {
            return
                $"{nameof(Litter)} {litter.Id}\n" +
                $"\n" +
                $"{_entityFormSettings.LitterProperties[nameof(litter.Letter)]}: {litter.Letter}\n" +
                $"{_entityFormSettings.LitterProperties[nameof(litter.BirthDay)]}: {litter.BirthDay}\n" +
                $"{_entityFormSettings.LitterProperties[nameof(litter.Description)]}: {litter.Description}\n" +
                $"\n" +
                $"Мама: {litter.MotherCat?.Name}\n" +
                $"Папа: {litter.FatherCat?.Name}\n" +
                $"\n" +
                $"Количество котят: {litter.Kittens.Count}";
        }

        private string CreateKittenFormMessage(Kitten kitten)
        {
            return
                $"{nameof(Kitten)} {kitten.Id}\n" +
                $"\n" +
                $"{_entityFormSettings.KittenProperties[nameof(kitten.Name)]}: {kitten.Name}\n" +
                $"{_entityFormSettings.KittenProperties[nameof(kitten.BirthDay)]}: {kitten.BirthDay.ToString(new CultureInfo("ru-RU"))}\n" +
                $"{_entityFormSettings.KittenProperties[nameof(kitten.IsMale)]}: {(kitten.IsMale ? "мужской" : "женский")}\n" +
                $"{_entityFormSettings.KittenProperties[nameof(kitten.Description)]}: {kitten.Description}\n" +
                $"{_entityFormSettings.KittenProperties[nameof(kitten.Class)]}: {kitten.Class}\n" +
                $"{_entityFormSettings.KittenProperties[nameof(kitten.Status)]}: {kitten.Status}\n" +
                $"Фотографии: {kitten.Photos.Count}/5";
        }

        #endregion

    }
}
