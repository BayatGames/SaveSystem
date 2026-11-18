using Bayat.Json.Linq;
using Bayat.Json.Serialization;
using System;
using UnityEngine;

namespace Bayat.Json.Converters
{
    public class ResolutionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializerWriter internalWriter)
        {
            var res = (Resolution)value;
            writer.WriteStartObject();
            writer.WritePropertyName("height");
            writer.WriteValue(res.height);
            writer.WritePropertyName("width");
            writer.WriteValue(res.width);
            writer.WritePropertyName("refreshRate");
#if UNITY_2022_2_OR_NEWER
            // Match the legacy integer API's rounding and JSON representation.
            writer.WriteValue((int)Math.Round(res.refreshRateRatio.value));
#else
            writer.WriteValue(res.refreshRate);
#endif
            writer.WriteEndObject();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Resolution);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializerReader internalReader)
        {
            var obj = JObject.Load(reader);

            var result = new Resolution
            {
                height = (int)obj["height"],
                width = (int)obj["width"],
#if UNITY_2022_2_OR_NEWER
                refreshRateRatio = new RefreshRate { numerator = unchecked((uint)(int)obj["refreshRate"]), denominator = 1 }
#else
                refreshRate = (int)obj["refreshRate"]
#endif
            };

            return result;
        }

        public override bool CanRead
        {
            get { return true; }
        }
    }
}
