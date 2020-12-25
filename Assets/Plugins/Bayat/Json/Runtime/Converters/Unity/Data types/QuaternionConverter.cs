using Bayat.Json.Serialization;
using System;
using UnityEngine;

namespace Bayat.Json.Converters
{

    public class QuaternionConverter : ObjectJsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Quaternion);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            Quaternion quaternion = (Quaternion)value;
            writer.WritePropertyName("x");
            writer.WriteValue(quaternion.x);
            writer.WritePropertyName("y");
            writer.WriteValue(quaternion.y);
            writer.WritePropertyName("z");
            writer.WriteValue(quaternion.z);
            writer.WritePropertyName("w");
            writer.WriteValue(quaternion.w);
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            Quaternion quaternion = (Quaternion)targetObject;
            switch (memberName)
            {
                case "x":
                    quaternion.x = (float)reader.ReadAsDecimal();
                    break;
                case "y":
                    quaternion.y = (float)reader.ReadAsDecimal();
                    break;
                case "z":
                    quaternion.z = (float)reader.ReadAsDecimal();
                    break;
                case "w":
                    quaternion.w = (float)reader.ReadAsDecimal();
                    break;
                default:
                    reader.Skip();
                    break;
            }
            return quaternion;
        }

    }

}