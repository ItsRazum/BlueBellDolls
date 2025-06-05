namespace BlueBellDolls.Common.Types
{
    public abstract class Cat : DisplayableEntityBase
    {
        #region Properties

        public string Name { get; set; }
        public DateOnly BirthDay { get; set; }
        public bool IsMale { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }

        #endregion

    }
}
