using BlueBellDolls.Common.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services
{
    public interface ICatColorTreeService
    {
        Task<bool> ForceUpdateCatColorTree(CancellationToken token = default);
        Task<CatColorTree?> GetCatColorTreeAsync(CancellationToken token = default);
    }
}