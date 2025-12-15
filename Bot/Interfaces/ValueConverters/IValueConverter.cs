namespace BlueBellDolls.Bot.Interfaces.ValueConverters
{
    public interface IValueConverter
    {
        object Convert(string value, Type targetType);
    }
}
