using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Reflection;

namespace BlueBellDolls.Bot.Services
{
    public class EntityFormService : IEntityFormService
    {
        private readonly EntityFormSettings _settings;
        private readonly IValueConverter _valueConverter;
        private readonly ConcurrentDictionary<Type, Dictionary<string, Action<object, string>>> _entityProperties;

        public EntityFormService(
            IOptions<EntityFormSettings> options,
            IValueConverter valueConverter)
        {
            _settings = options.Value;
            _valueConverter = valueConverter;
            _entityProperties = new();

            InitializePropertyMappings();
        }

        private void InitializePropertyMappings()
        {
            AddEntityProperties<Kitten>(_settings.KittenProperties);
            AddEntityProperties<ParentCat>(_settings.ParentCatProperties);
            AddEntityProperties<Litter>(_settings.LitterProperties);
        }

        private void AddEntityProperties<TEntity>(Dictionary<string, string> propertyMappings)
        {
            var entityType = typeof(TEntity);
            var propertyActions = new Dictionary<string, Action<object, string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var (propertyName, displayName) in propertyMappings)
            {
                var propertyInfo = entityType.GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy) ?? throw new InvalidOperationException($"Свойство '{propertyName}' не найдено в типе '{entityType.Name}'.");
                propertyActions[displayName] = (entity, value) =>
                {
                    var convertedValue = _valueConverter.Convert(value, propertyInfo.PropertyType);
                    propertyInfo.SetValue(entity, convertedValue);
                };
            }

            _entityProperties[entityType] = propertyActions;
        }

        public void UpdateProperty<TEntity>(TEntity entity, string displayName, string value) where TEntity : IEntity
        {
            ArgumentNullException.ThrowIfNull(entity);
            UpdateEntity(entity, displayName, value);
        }

        public void UpdateProperty(IEntity entity, string displayName, string value)
        {
            UpdateEntity(entity, displayName, value);
        }

        private void UpdateEntity(IEntity entity, string displayName, string value) 
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Отображаемое имя свойства не может быть пустым.", nameof(displayName));

            var entityType = entity.GetType();
            if (!_entityProperties.TryGetValue(entityType, out var propertyActions))
                throw new InvalidOperationException($"Настройки для типа '{entityType.Name}' не найдены.");

            if (!propertyActions.TryGetValue(displayName, out var updateAction))
                throw new InvalidOperationException($"Отображаемое имя '{displayName}' не сопоставлено ни с одним свойством в типе '{entityType.Name}'.");

            updateAction(entity!, value);
        }
    }
}
