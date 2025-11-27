using BlueBellDolls.Common.Types;

namespace BlueBellDolls.Common.Models
{
    public class CatColor : DisplayableEntityBase
    {
        public string Identifier { get; set; }

        public string Description { get; set; }

        public override string DisplayName => Identifier;
    }
}
