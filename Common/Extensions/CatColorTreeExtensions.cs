using BlueBellDolls.Common.Dtos;

namespace BlueBellDolls.Common.Extensions
{
    public static class CatColorTreeExtensions
    {
        public static string[] ToTreeMap(this CatColorTree catColorTree)
        {
            var result = new List<string>(3);
            if (catColorTree.Count == 0)
                return [];

            foreach (var colorPair in catColorTree)
                foreach (var patternPair in colorPair.Value)
                    foreach (var shade in patternPair.Value)
                        result.Add($"{colorPair.Key}{patternPair.Key}{(shade == "/" ? "" : shade)}".Trim());

            return [.. result];
        }
    }
}
