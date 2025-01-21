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
    }
}