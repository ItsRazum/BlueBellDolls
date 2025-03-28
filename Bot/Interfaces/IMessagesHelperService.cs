using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IMessagesHelperService
    {
        Task SendGeneticTestsManagementMessageAsync(Chat chat, ParentCat entity, CancellationToken token = default);
        Task SendPhotoManagementMessageAsync(Chat chat, IDisplayableEntity entity, CancellationToken token = default);
        Task SendTitlesManagementMessageAsync(Chat chat, ParentCat entity, CancellationToken token = default);

        Task SendDeletePhotosConfirmationAsync(CallbackQueryAdapter c, IDisplayableEntity entity, CancellationToken token = default);
        Task SendDeleteTitlesConfirmationAsync(CallbackQueryAdapter c, ParentCat entity, CancellationToken token = default);
        Task SendDeleteGeneticTestsConfirmationAsync(CallbackQueryAdapter c, ParentCat entity, CancellationToken token = default);
    }
}