using BlueBellDolls.Bot.Interfaces.Services.Management.Base;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Management
{
    public interface ILitterManagementService : IDisplayableEntityManagementService<Litter>
    {
        Task<ServiceResult<Kitten>> AddNewKittenToLitterAsync(int litterId, CancellationToken token);
        Task<ServiceResult<StructWrapper<(bool isMale, Litter litter)>>> SetParentCatForLitterAsync(int litterId, int parentCatId, CancellationToken token);
    }
}