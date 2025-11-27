namespace BlueBellDolls.Bot.Interfaces
{
    public interface IArgumentParseHelperService
    {
        (IEnumerable<int> photoIds, IEnumerable<int> photoMessageIds) ParsePhotosArgs(string data);
    }
}