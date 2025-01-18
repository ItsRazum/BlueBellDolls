using BlueBellDolls.Common.Types;

namespace BlueBellDolls.Common.Models
{
    public class ParentCat : Cat
    {

        #region Properties

        public string GeneticTestOne { get; set; }
        public string GeneticTestTwo { get; set; }
        public List<string> Titles { get; set; }
        public string? OldDescription { get; set; }

        #endregion

    }
}
