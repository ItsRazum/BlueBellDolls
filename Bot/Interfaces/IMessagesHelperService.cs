using BlueBellDolls.Common.Interfaces;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IMessagesHelperService
    {
        Task SendPhotoManagementMessageAsync(Chat chat, IDisplayableEntity entity, CancellationToken token = default);
    }
}