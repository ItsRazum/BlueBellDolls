namespace BlueBellDolls.Bot.Settings
{
    public class EntitySettings
    {
        public Dictionary<string, int> MaxPhotos { get; set; }
        public int MaxParentCatTitlesCount { get; set; }
        public int MaxParentCatGeneticTestsCount { get; set; }

        public Dictionary<string, Dictionary<string, string[]>> CatColors { get; set; }
    }
}
