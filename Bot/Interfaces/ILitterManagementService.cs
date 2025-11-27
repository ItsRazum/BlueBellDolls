using BlueBellDolls.Bot.Records;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface ILitterManagementService : IDisplayableEntityManagementService<Litter>
    {
        Task<ManagementOperationResult<Kitten>> AddNewKittenToLitterAsync(int litterId, CancellationToken token);
        Task<ManagementOperationResult> SetParentCatForLitterAsync(int litterId, int parentCatId, CancellationToken token);
    }
}