using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AgolPlugin.Converters.Json
{
    public class UnixTimestampToDateTimeConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {

            if (reader.TokenType == JsonToken.Null) return null;
            if (reader.TokenType != JsonToken.Integer) return null;

            return long.TryParse(reader.Value.ToString(), out var epoch)
                ? DateTimeOffset.FromUnixTimeMilliseconds(epoch).DateTime.ToLocalTime()
                : (DateTime?)null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public bool TryConvert(string value, out DateTime dateTime)
        {
            dateTime = DateTime.MinValue;

            var dt = long.TryParse(value, out var epoch)
                ? DateTimeOffset.FromUnixTimeMilliseconds(epoch).DateTime.ToLocalTime()
                : (DateTime?)null;

            if(dt != null)
                dateTime = (DateTime)dt;

            return true;
        }
    }
}