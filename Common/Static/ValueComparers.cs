using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BlueBellDolls.Common.Static
{
    internal static class ValueComparers
    {
        /// <summary>
        /// ValueComparer для List<string>.
        /// </summary>
        public static ValueComparer<List<string>> ListStringComparer { get; } = 
            new ValueComparer<List<string>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());
    }
}
