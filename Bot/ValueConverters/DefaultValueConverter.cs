using BlueBellDolls.Bot.Interfaces.ValueConverters;
using System.Globalization;

namespace BlueBellDolls.Bot.ValueConverters
{
    public class EntityValueConverter : IValueConverter
    {
        public object Convert(string value, Type targetType)
        {
            if (targetType.IsEnum)
                return Enum.Parse(targetType, value, true);

            if (targetType == typeof(DateTime))
                return DateTime.Parse(value);

            if (targetType == typeof(DateOnly))
                return DateOnly.Parse(value, new CultureInfo("ru-RU"));

            if (targetType == typeof(bool))
            {
                return value switch
                {
                    "мужской" => true,
                    "женский" => false,
                    _ => throw new FormatException("Неверный формат пола."),
                };
            }

            if (targetType == typeof(int))
                return int.Parse(value);

            return System.Convert.ChangeType(value, targetType);
        }
    }
}
