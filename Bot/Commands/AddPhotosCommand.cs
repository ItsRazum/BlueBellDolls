using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;

namespace BlueBellDolls.Bot.Commands
{
    public class AddPhotosCommand : CommandHandler<MessageAdapter>
    {
        public AddPhotosCommand(
            IBotService botService) 
            : base(botService)
        {
            Handlers.Add("Фото", HandleCommandAsync);
        }

        private async Task HandleCommandAsync(MessageAdapter m, CancellationToken token)
        {

        }
    }
}