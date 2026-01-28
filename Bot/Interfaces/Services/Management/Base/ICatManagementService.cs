using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Common.Types;

namespace BlueBellDolls.Bot.Interfaces.Services.Management.Base
{
    public interface ICatManagementService<TEntity> : IDisplayableEntityManagementService<TEntity> where TEntity : Cat
    {
        Task<ServiceResult<TEntity>> UpdateColorAsync(int entityId, string color, CancellationToken token);
    }
}
