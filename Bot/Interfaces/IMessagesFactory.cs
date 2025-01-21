using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Interfaces
{
    internal interface IMessagesFactory
    {
        string GetStartMessage();

        string GetParentCatFormMessage(ParentCat parentCat);
    }
}