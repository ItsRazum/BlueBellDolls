using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IKeyboardsProvider
    {
        InlineKeyboardMarkup CreateEntityListKeyboard<TEntity>(
            IEnumerable<TEntity> entities, 
            ListUnitActionMode actionMode = ListUnitActionMode.Edit, 
            (int page, int totalPagesCount)? pageParameters = null, 
            IEntity? unitOwner = null) 
            where TEntity : class, IDisplayableEntity;

        InlineKeyboardMarkup CreateEntityOptionsKeyboard(IDisplayableEntity entity);

        InlineKeyboardMarkup CreateConfirmEntityDeletionKeyboard(string callback, IDisplayableEntity entity, string onDeletionCanceledCallback, params string[] callbacksAfterDeletion);

        InlineKeyboardMarkup CreateEntityFromLitterKeyboard(IDisplayableEntity entity, int litterId);
    }
}
