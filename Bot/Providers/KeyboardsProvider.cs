using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Bot.Providers
{
    public class KeyboardsProvider : IKeyboardsProvider
    {
        #region Fields

        private readonly Dictionary<Type, Func<IDisplayableEntity, InlineKeyboardMarkup>> _entityOptionsKeyboards;
        private readonly ICallbackDataProvider _callbackDataProvider;

        #endregion

        #region Constructor

        public KeyboardsProvider(ICallbackDataProvider callbackDataProvider)
        {
            _callbackDataProvider = callbackDataProvider;

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
            (int page, int totalPagesCount)? pageParameters = null,
            IEntity? unitOwner = null)
            where TEntity : class, IDisplayableEntity
        {
            var result = new InlineKeyboardMarkup();
            if (unitOwner != null)
                result.AddNewRow(CreateBackToLitterButton(unitOwner.Id));

            foreach (var entity in entities)
                result.AddNewRow(CreateListEntityReferenceButton(entity, actionMode, unitOwner));

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

            return result;
        }

        public InlineKeyboardMarkup CreateEntityPhotosKeyboard(IDisplayableEntity entity, int[] photoMessageIds, int[]? selectedPhotosIndexes = null)
        {
            var result = new InlineKeyboardMarkup(CreateBackToFormButton(entity, _callbackDataProvider.CreateDeleteMessagesCallback(photoMessageIds)));
            for (int i = 0; i < 5; i++)
                result.AddNewRow(CreatePhotoReferenceButton(entity, i, !selectedPhotosIndexes?.Contains(i) ?? true));

            if (selectedPhotosIndexes?.Length > 0)
            {
                if (selectedPhotosIndexes.Length == 1)
                    result.AddNewRow(CreateSelectPhotoAsDefaultButton(entity, selectedPhotosIndexes.First()));

                result.AddNewRow(CreateDeletePhotosButton(entity));
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
                        _callbackDataProvider.CreateSelectParentCatCallback(parentCat.IsMale, litterId)
                        )
                    );

            result.AddNewRow(CreateDeleteButton(entity, _callbackDataProvider.CreateDeleteEntityCallback(entity, litterId)));

            return result;
        }

        #endregion

        #region Private Methods

        private InlineKeyboardMarkup CreateLitterOptionsKeyboard(Litter litter)
        {
            var result = CreateEntityListKeyboard(litter.Kittens);

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
                    : _callbackDataProvider.CreateSelectParentCatCallback(false, litter.Id)
            );

            (string fatherButtonText, string fatherButtonCallbackData) = (
                "Папа",
                litter.FatherCat != null
                    ? _callbackDataProvider.CreateOpenEntityInLitterCallback(litter.FatherCat, litter.Id)
                    : _callbackDataProvider.CreateSelectParentCatCallback(true, litter.Id)
            );

            result.AddNewRow(
            [
                InlineKeyboardButton.WithCallbackData(motherButtonText, motherButtonCallbackData),
            InlineKeyboardButton.WithCallbackData(fatherButtonText, fatherButtonCallbackData)
            ]);

            result.AddNewRow(CreateDeleteButton(litter, _callbackDataProvider.CreateDeleteEntityCallback(litter)));

            return result;
        }

        private InlineKeyboardMarkup CreateParentCatOptions(ParentCat parentCat)
        {
            var result = new InlineKeyboardMarkup();
            result.AddNewRow(CreateDeleteButton(parentCat, _callbackDataProvider.CreateDeleteEntityCallback(parentCat)));

            if (parentCat.Photos.Count > 0)
                result.AddNewRow(CreateManagePhotosButton(parentCat));

            if (parentCat.Titles.Count > 0)
                result.AddNewRow(CreateManageTitlesButton(parentCat));

            return result;
        }

        private InlineKeyboardMarkup CreateKittenOptions(Kitten kitten)
        {
            var result = new InlineKeyboardMarkup();
            result.AddNewRow(CreateBackToLitterButton(kitten.LitterId));
            result.AddNewRow(CreateDeleteButton(kitten, _callbackDataProvider.CreateDeleteEntityCallback(kitten, kitten.LitterId)));
            return result;
        }

        private InlineKeyboardButton[] CreatePageControlsButtons<TEntity>((int page, int totalPagesCount) pageParameters)
        {
            var entityName = typeof(TEntity).Name;
            var pageCounterString = pageParameters.totalPagesCount == 0
                ? "Список пуст!"
                : $"{pageParameters.page}/{pageParameters.totalPagesCount}";
            var buttons = new List<InlineKeyboardButton?>
        {
            pageParameters.page > 1
                ? InlineKeyboardButton.WithCallbackData("«", _callbackDataProvider.CreateListEntityCallback(entityName, pageParameters.page - 1))
                : null,
            InlineKeyboardButton.WithCallbackData(pageCounterString, _callbackDataProvider.CreateListEntityCallback(entityName, pageParameters.page)),
            pageParameters.page < pageParameters.totalPagesCount
                ? InlineKeyboardButton.WithCallbackData("»", _callbackDataProvider.CreateListEntityCallback(entityName, pageParameters.page + 1))
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
                entity.DisplayName,
                _callbackDataProvider.CreateEntityReferenceCallback(entity, actionMode, unitOwner)
            );
        }

        private InlineKeyboardButton CreateAddButton<TEntity>() where TEntity : class, IDisplayableEntity
            => InlineKeyboardButton.WithCallbackData(
                "Добавить ещё",
                _callbackDataProvider.CreateAddEntityCallback(typeof(TEntity).Name)
            );

        private InlineKeyboardButton CreateDeleteYesButton(string callback, params string[] callbacksAfterDeletion)
            => InlineKeyboardButton.WithCallbackData("Да", $"{_callbackDataProvider.CreateConfirmCallback(callback)}\n{string.Join('\n', callbacksAfterDeletion)}");

        private InlineKeyboardButton CreateDeleteNoButton(string callback)
            => InlineKeyboardButton.WithCallbackData("Нет", callback);

        private InlineKeyboardButton CreateManagePhotosButton(IDisplayableEntity entity)
            => InlineKeyboardButton.WithCallbackData("Редактировать фото", _callbackDataProvider.CreateAddPhotosCallback(entity));

        private InlineKeyboardButton CreateManageTitlesButton(IDisplayableEntity entity)
            => InlineKeyboardButton.WithCallbackData("Редактировать титулы", _callbackDataProvider.CreateAddTitlesCallback(entity));

        private InlineKeyboardButton CreatePhotoReferenceButton(IDisplayableEntity entity, int number, bool select)
        {
            var selectionChar = select ? "⚫️" : "🟢";

            return InlineKeyboardButton.WithCallbackData(
                $"{selectionChar}Фото {number + 1}",
                _callbackDataProvider.CreateTogglePhotoSelectionCallback(entity, number, select));
        }

        private InlineKeyboardButton CreateSelectPhotoAsDefaultButton(IDisplayableEntity entity, int photoIndex)
            => InlineKeyboardButton.WithCallbackData(
                "Сделать основным", 
                _callbackDataProvider.CreateMakeDefaultPhotoForEntityCallback(entity, photoIndex));

        private InlineKeyboardButton CreateDeletePhotosButton(IDisplayableEntity entity)
            => InlineKeyboardButton.WithCallbackData(
                "Удалить выбранное", 
                _callbackDataProvider.CreateDeletePhotosForEntityCallback(entity));

        private InlineKeyboardButton CreateBackToFormButton(IDisplayableEntity entity, string? callbackBeforeRedirect = null)
            => InlineKeyboardButton.WithCallbackData(
                "Назад",
                string
                .Join('\n', callbackBeforeRedirect, _callbackDataProvider.CreateEditEntityCallback(entity))
                .TrimStart('\n'));

        #endregion
    }
}