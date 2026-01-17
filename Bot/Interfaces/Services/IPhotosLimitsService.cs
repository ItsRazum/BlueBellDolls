using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Interfaces.Services
{
    public interface IPhotosLimitsService
    {
        int GetLimit<TEntity>(PhotosType photosType) where TEntity : class, IDisplayableEntity;
    }
}