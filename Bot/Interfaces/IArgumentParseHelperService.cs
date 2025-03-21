namespace BlueBellDolls.Bot.Interfaces
{
    public interface IArgumentParseHelperService
    {
        (IEnumerable<int> photoIndexes, IEnumerable<int> photoMessageIds) ParsePhotosArgs(string data);
    }
}