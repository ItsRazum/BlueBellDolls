namespace BlueBellDolls.Common.Types
{
    public class PhotosLimitsDictionary : Dictionary<string, int>
    {
        public int SafeGet(string key)
            => TryGetValue(key, out var value) ? value : 0;
    }
}
