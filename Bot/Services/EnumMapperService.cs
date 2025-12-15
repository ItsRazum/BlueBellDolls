using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Common.Enums;

namespace BlueBellDolls.Bot.Services
{
    public class EnumMapperService : IEnumMapperService
    {
        public string GetMapping(KittenStatus status, bool isMale = true)
        {
            return status switch
            {
                KittenStatus.Available => "Доступ" + (isMale ? "ен" : "на"),
                KittenStatus.Sold => "Продан" + (isMale ? string.Empty : "а"),
                KittenStatus.UnderObservation => "Под наблюдением",
                KittenStatus.Reserved => "Зарезервирован" + (isMale ? string.Empty : "а"),
                _ => status.ToString(),
            };
        }
    }
}
