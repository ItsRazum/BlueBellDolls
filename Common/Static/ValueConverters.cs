using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace BlueBellDolls.Common.Static
{
    internal static class ValueConverters
    {
        /// <summary>
        /// ValueConverter для сериализации и десериализации List<string> в JSON.
        /// </summary>
        public static ValueConverter<List<string>, string> ListStringConverter { get; } =
            new ValueConverter<List<string>, string>(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v)!
                );

        /// <summary>
        /// ValueConverter для сериализации и десериализации Dictionary<string, string> в JSON.
        /// </summary>
        public static ValueConverter<Dictionary<string, string>, string> DictionaryStringConverter { get; } =
            new ValueConverter<Dictionary<string, string>, string>(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v)!);
    }
}