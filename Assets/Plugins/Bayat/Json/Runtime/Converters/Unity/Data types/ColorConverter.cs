using System;

using Bayat.Json.Serialization;

using UnityEngine;

namespace Bayat.Json.Converters
{

    public class ColorConverter : ObjectJsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color) || objectType == typeof(Color32);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            if (value is Color32)
            {
                Color32 color = (Color32)value;
                writer.WritePropertyName("a");
                writer.WriteValue(color.a);
                writer.WritePropertyName("r");
                writer.WriteValue(color.r);
                writer.WritePropertyName("g");
                writer.WriteValue(color.g);
                writer.WritePropertyName("b");
                writer.WriteValue(color.b);
            }
            else
            {
                Color color = (Color)value;
                writer.WritePropertyName("a");
                writer.WriteValue(color.a);
                writer.WritePropertyName("r");
                writer.WriteValue(color.r);
                writer.WritePropertyName("g");
                writer.WriteValue(color.g);
                writer.WritePropertyName("b");
                writer.WriteValue(color.b);
            }
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            if (objectType == typeof(Color32))
            {
                Color32 color = (Color32)targetObject;
                switch (memberName)
                {
                    case "a":
                        color.a = (byte)reader.ReadAsInt32();
                        break;
                    case "r":
                        color.r = (byte)reader.ReadAsInt32();
                        break;
                    case "g":
                        color.g = (byte)reader.ReadAsInt32();
                        break;
                    case "b":
                        color.b = (byte)reader.ReadAsInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
                return color;
            }
            else
            {
                Color color = (Color)targetObject;
                switch (memberName)
                {
                    case "a":
                        color.a = (float)reader.ReadAsDecimal();
                        break;
                    case "r":
                        color.r = (float)reader.ReadAsDecimal();
                        break;
                    case "g":
                        color.g = (float)reader.ReadAsDecimal();
                        break;
                    case "b":
                        color.b = (float)reader.ReadAsDecimal();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
                return color;
            }
        }

    }

}