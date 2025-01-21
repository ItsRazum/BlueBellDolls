using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Common.Models;
using System.Globalization;

namespace BlueBellDolls.Bot.Factories
{
    public class MessagesFactory : IMessagesFactory
    {
        public string GetStartMessage()
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

        public string GetParentCatFormMessage(ParentCat parentCat)
        {
            return
                $"ParentCat {parentCat.Id}\n" +
                $"\n" +
                $"Имя: {parentCat.Name}\n" +
                $"Дата рождения: {parentCat.BirthDay.ToString(new CultureInfo("ru-RU"))}\n" +
                $"Пол: {(parentCat.IsMale ? "мужской" : "женский")}\n" +
                $"Описание: {parentCat.Description}\n" +
                $"Фотографии: {parentCat.Photos.Count}/5\n" +
                $"Титулы: {parentCat.Titles.Count}/6\n" +
                $"Генетический тест 1: {(!string.IsNullOrWhiteSpace(parentCat.GeneticTestOne) ? "получен" : "нужно отправить")}\n" +
                $"Генетический тест 2: {(!string.IsNullOrWhiteSpace(parentCat.GeneticTestTwo) ? "получен" : "нужно отправить")}";
        }
    }
}
