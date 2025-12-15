using BlueBellDolls.Common.Enums;

namespace BlueBellDolls.Bot.Interfaces.Services
{
    public interface IEnumMapperService
    {
        string GetMapping(KittenStatus status, bool isMale = true);
    }
}