using BlueBellDolls.Bot.Interfaces;

namespace BlueBellDolls.Bot.Services
{
    public class ArgumentParseHelperService : IArgumentParseHelperService
    {
        public (IEnumerable<int> photoIndexes, IEnumerable<int> photoMessageIds) ParsePhotosArgs(string data)
        {
            var key = data.Split(" : ");

            var photoIndexesString = key.First();
            var photoMessageIds = key.Last().Split(", ").Select(int.Parse);

            var photoIndexes = (photoIndexesString != "-" ? photoIndexesString.Split(", ")
                .Select(int.Parse) : [])
                .Order();

            return (photoIndexes, photoMessageIds);
        }
    }
}
