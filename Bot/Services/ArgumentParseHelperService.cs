using BlueBellDolls.Bot.Interfaces;

namespace BlueBellDolls.Bot.Services
{
    public class ArgumentParseHelperService : IArgumentParseHelperService
    {
        public (IEnumerable<int> photoIds, IEnumerable<int> photoMessageIds) ParsePhotosArgs(string data)
        {
            var key = data.Split(" : ");

            var photoIdsString = key.First();
            var photoMessageIds = key.Last().Split(", ").Select(int.Parse);

            var photoIds = photoIdsString != "-" ? photoIdsString.Split(", ").Select(int.Parse) : [];

            return (photoIds, photoMessageIds);
        }
    }
}
