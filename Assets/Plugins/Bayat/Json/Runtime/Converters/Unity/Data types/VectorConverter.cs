using Bayat.Json.Serialization;
using System;
using UnityEngine;

namespace Bayat.Json.Converters
{

    public class VectorConverter : ObjectJsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2) || objectType == typeof(Vector3) || objectType == typeof(Vector4)
#if UNITY_2017_2_OR_NEWER
                || objectType == typeof(Vector2Int) || objectType == typeof(Vector3Int)
#endif
                ;
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            if (objectType == typeof(Vector2))
            {
                var vector = (Vector2)value;
                WriteVector(writer, vector.x, vector.y, null, null);
            }
            else if (objectType == typeof(Vector3))
            {
                var vector = (Vector3)value;
                WriteVector(writer, vector.x, vector.y, vector.z, null);
            }
            else if (objectType == typeof(Vector4))
            {
                var vector = (Vector4)value;
                WriteVector(writer, vector.x, vector.y, vector.z, vector.w);
            }
#if UNITY_2017_2_OR_NEWER
            else if (objectType == typeof(Vector2Int))
            {
                var vector = (Vector2Int)value;
                WriteVectorInt(writer, vector.x, vector.y, null);
            }
            else if (objectType == typeof(Vector3Int))
            {
                var vector = (Vector3Int)value;
                WriteVectorInt(writer, vector.x, vector.y, vector.z);
            }
#endif
        }

        protected virtual void WriteVector(JsonWriter writer, float x, float y, float? z, float? w)
        {
            writer.WritePropertyName("x");
            writer.WriteValue(x);
            writer.WritePropertyName("y");
            writer.WriteValue(y);
            if (z.HasValue)
            {
                writer.WritePropertyName("z");
                writer.WriteValue(z.Value);

                if (w.HasValue)
                {
                    writer.WritePropertyName("w");
                    writer.WriteValue(w.Value);
                }
            }
        }

#if UNITY_2017_2_OR_NEWER
        protected virtual void WriteVectorInt(JsonWriter writer, int x, int y, int? z)
        {
            writer.WritePropertyName("x");
            writer.WriteValue(x);
            writer.WritePropertyName("y");
            writer.WriteValue(y);
            if (z.HasValue)
            {
                writer.WritePropertyName("z");
                writer.WriteValue(z.Value);
            }
        }
#endif

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            if (objectType == typeof(Vector2))
            {
                Vector2 vector = (Vector2)targetObject;
                switch (memberName)
                {
                    case "x":
                        vector.x = (float)reader.ReadAsDecimal();
                        break;
                    case "y":
                        vector.y = (float)reader.ReadAsDecimal();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
                return vector;
            }
            else if (objectType == typeof(Vector3))
            {
                Vector3 vector = (Vector3)targetObject;
                switch (memberName)
                {
                    case "x":
                        vector.x = (float)reader.ReadAsDecimal();
                        break;
                    case "y":
                        vector.y = (float)reader.ReadAsDecimal();
                        break;
                    case "z":
                        vector.z = (float)reader.ReadAsDecimal();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
                return vector;
            }
            else if (objectType == typeof(Vector4))
            {
                Vector4 vector = (Vector4)targetObject;
                switch (memberName)
                {
                    case "x":
                        vector.x = (float)reader.ReadAsDecimal();
                        break;
                    case "y":
                        vector.y = (float)reader.ReadAsDecimal();
                        break;
                    case "z":
                        vector.z = (float)reader.ReadAsDecimal();
                        break;
                    case "w":
                        vector.w = (float)reader.ReadAsDecimal();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
                return vector;
            }
#if UNITY_2017_2_OR_NEWER
            else if (objectType == typeof(Vector2Int))
            {
                Vector2Int vector = (Vector2Int)targetObject;
                switch (memberName)
                {
                    case "x":
                        vector.x = reader.ReadAsInt32().GetValueOrDefault();
                        break;
                    case "y":
                        vector.y = reader.ReadAsInt32().GetValueOrDefault();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
                return vector;
            }
            else if (objectType == typeof(Vector3Int))
            {
                Vector3Int vector = (Vector3Int)targetObject;
                switch (memberName)
                {
                    case "x":
                        vector.x = reader.ReadAsInt32().GetValueOrDefault();
                        break;
                    case "y":
                        vector.y = reader.ReadAsInt32().GetValueOrDefault();
                        break;
                    case "z":
                        vector.z = reader.ReadAsInt32().GetValueOrDefault();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
                return vector;
            }
#endif
            return targetObject;
        }

    }

}