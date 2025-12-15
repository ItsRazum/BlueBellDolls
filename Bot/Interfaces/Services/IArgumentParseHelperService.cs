namespace BlueBellDolls.Bot.Interfaces.Services
{
    public interface IArgumentParseHelperService
    {
        (IEnumerable<int> photoIds, IEnumerable<int> photoMessageIds) ParsePhotosArgs(string data);
    }
}