using BlueBellDolls.Bot.Settings;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Reflection;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using CatColor = BlueBellDolls.Common.Models.CatColor;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Interfaces.ValueConverters;

namespace BlueBellDolls.Bot.Services
{
    public class EntityFormService : IEntityFormService
    {
        private readonly EntityFormSettings _settings;
        private readonly IValueConverter _valueConverter;
        private readonly Dictionary<Type, Dictionary<string, string>> _entityPropsMappings;
        private readonly ConcurrentDictionary<Type, Dictionary<string, Action<object, string>>> _entityProperties;
        private readonly ConcurrentDictionary<Type, Dictionary<string, Type>> _entityPropertyTypes;

        public EntityFormService(
            IOptions<EntityFormSettings> options,
            IValueConverter valueConverter)
        {
            _settings = options.Value;
            _valueConverter = valueConverter;
            _entityProperties = new();
            _entityPropertyTypes = new();
            _entityPropsMappings = [];

            InitializePropertyMappings();
        }

        private void InitializePropertyMappings()
        {
            AddEntityProperties<Kitten>(_settings.KittenProperties);
            AddEntityProperties<ParentCat>(_settings.ParentCatProperties);
            AddEntityProperties<Litter>(_settings.LitterProperties);
            AddEntityProperties<CatColor>(_settings.CatColorProperties);
        }

        private void AddEntityProperties<TEntity>(Dictionary<string, string> propertyMappings)
        {
            var entityType = typeof(TEntity);
            _entityPropsMappings.Add(entityType, propertyMappings);

            var propertyActions = new Dictionary<string, Action<object, string>>(StringComparer.OrdinalIgnoreCase);
            var propertyTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            foreach (var (propertyName, displayName) in propertyMappings)
            {
                var propertyInfo = entityType.GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy)
                    ?? throw new InvalidOperationException($"Свойство '{propertyName}' не найдено в типе '{entityType.Name}'.");

                propertyActions[displayName] = (entity, value) =>
                {
                    var convertedValue = _valueConverter.Convert(value, propertyInfo.PropertyType);
                    propertyInfo.SetValue(entity, convertedValue);
                };

                propertyTypes[displayName] = propertyInfo.PropertyType;
            }

            _entityProperties[entityType] = propertyActions;
            _entityPropertyTypes[entityType] = propertyTypes;
        }

        public string? GetPropertyName<TEntity>(string key) where TEntity : IEntity
        {
            ArgumentNullException.ThrowIfNull(key);
            if (_entityPropsMappings.TryGetValue(typeof(TEntity), out var mappings))
            {
                var pair = mappings.FirstOrDefault(kvp => kvp.Value == key);
                return pair.Key;
            }

            return null;
        }

        public Type? GetPropertyType<TEntity>(string displayName) where TEntity : IEntity
        {
            ArgumentNullException.ThrowIfNull(displayName);
            if (_entityPropertyTypes.TryGetValue(typeof(TEntity), out var types)
                && types.TryGetValue(displayName, out var type))
                return type;

            return null;
        }

        public bool UpdateProperty<TEntity>(TEntity entity, string displayName, string value) where TEntity : IEntity
        {
            ArgumentNullException.ThrowIfNull(entity);
            return UpdateEntity(entity, displayName, value);
        }

        private bool UpdateEntity(IEntity entity, string displayName, string value)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Отображаемое имя свойства не может быть пустым.", nameof(displayName));

            var entityType = entity.GetType();
            if (!_entityProperties.TryGetValue(entityType, out var propertyActions))
                return false;

            if (!propertyActions.TryGetValue(displayName, out var updateAction))
                return false;

            updateAction(entity!, value);
            return true;
        }
    }
}