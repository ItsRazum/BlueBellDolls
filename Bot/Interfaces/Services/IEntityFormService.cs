using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Interfaces.Services
{
    public interface IEntityFormService
    {
        string? GetPropertyName<TEntity>(string key) where TEntity : IEntity;
        bool UpdateProperty<TEntity>(TEntity entity, string displayName, string value) where TEntity : IEntity;
    }
}
