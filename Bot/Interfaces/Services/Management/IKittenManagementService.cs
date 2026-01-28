using BlueBellDolls.Bot.Interfaces.Services.Management.Base;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Management
{
    public interface IKittenManagementService : ICatManagementService<Kitten>
    {
        Task<ServiceResult<Kitten>> UpdateClassAsync(int entityId, KittenClass newClass, CancellationToken token = default);
        Task<ServiceResult<Kitten>> UpdateStatusAsync(int entityId, KittenStatus newStatus, CancellationToken token = default);
    }
}