using BlueBellDolls.Bot.Interfaces.Management.Base;
using BlueBellDolls.Bot.Records;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Management
{
    public interface IKittenManagementService : ICatManagementService<Kitten>
    {
        Task<ManagementOperationResult<Kitten>> UpdateClassAsync(int entityId, KittenClass newClass, CancellationToken token = default);
        Task<ManagementOperationResult<Kitten>> UpdateStatusAsync(int entityId, KittenStatus newStatus, CancellationToken token = default);
    }
}