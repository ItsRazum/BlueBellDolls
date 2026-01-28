using BlueBellDolls.Bot.Interfaces.Services.Management.Base;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Management
{
    public interface ICatColorManagementService : IDisplayableEntityManagementService<CatColor>
    {
        Task<ServiceResult<CatColor>> GetEntityAsync(string colorIdentifier, CancellationToken token = default);
    }
}