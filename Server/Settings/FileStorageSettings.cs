using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Types;

namespace BlueBellDolls.Server.Settings
{

    public class FileStorageSettings
    {
        public Dictionary<PhotosType, PhotosLimitsDictionary> Limits { get; set; }
    }
}
