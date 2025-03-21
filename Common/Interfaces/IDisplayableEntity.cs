namespace BlueBellDolls.Common.Interfaces
{
    public interface IDisplayableEntity : IEntity
    {
        string DisplayName { get; }

        Dictionary<string, string> Photos { get; set; }

        string[] PhotoIds => [ ..Photos.Keys ];
        string[] PhotoBase64Strings => [ ..Photos.Values ];
    }
}
