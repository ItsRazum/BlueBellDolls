namespace BlueBellDolls.Bot.Interfaces
{
    public interface IValueConverter
    {
        object Convert(string value, Type targetType);
    }
}
