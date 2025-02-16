using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IEntityFormService
    {
        bool UpdateProperty<TEntity>(TEntity entity, string displayName, string value) where TEntity : IEntity;

        public bool UpdateProperty(IEntity entity, string displayName, string value);
    }
}
