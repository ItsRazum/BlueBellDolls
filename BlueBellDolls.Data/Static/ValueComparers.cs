using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BlueBellDolls.Data.Static
{
    internal static class ValueComparers
    {
        /// <summary>
        /// ValueComparer для List<string>.
        /// </summary>
        public static ValueComparer<List<string>> ListStringComparer { get; } = 
            new(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

        public static ValueComparer<Dictionary<string, string>> DictionaryStringComparer { get; } =
            new(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToDictionary());
    }
}
