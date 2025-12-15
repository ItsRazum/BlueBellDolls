using BlueBellDolls.Bot.Interfaces.Management.Base;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Interfaces.Management
{
    public interface ICatColorManagementService : IDisplayableEntityManagementService<CatColor>
    {
        Task<CatColor?> GetEntityAsync(string colorIdentifier, CancellationToken token = default);
    }
}