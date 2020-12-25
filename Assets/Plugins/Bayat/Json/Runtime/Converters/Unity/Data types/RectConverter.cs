using Bayat.Json.Serialization;
using System;
using UnityEngine;

namespace Bayat.Json.Converters
{

    public class RectConverter : ObjectJsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Rect)
#if UNITY_2017_2_OR_NEWER
                || objectType == typeof(RectInt)
#endif
                ;
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            if (objectType == typeof(Rect))
            {
                var rect = (Rect)value;
                internalWriter.SerializeProperty(writer, "position", rect.position);
                internalWriter.SerializeProperty(writer, "size", rect.size);
            }
#if UNITY_2017_2_OR_NEWER
            else if (objectType == typeof(Vector2Int))
            {
                var rect = (RectInt)value;
                internalWriter.SerializeProperty(writer, "position", rect.position);
                internalWriter.SerializeProperty(writer, "size", rect.size);
            }
#endif
        }


        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            if (objectType == typeof(Rect))
            {
                var rect = (Rect)targetObject;
                switch (memberName)
                {
                    case "position":
                        rect.position = internalReader.Deserialize<Vector2>(reader);
                        break;
                    case "size":
                        rect.size = internalReader.Deserialize<Vector2>(reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
                return rect;
            }
            else
            {
                var rect = (RectInt)targetObject;
                switch (memberName)
                {
                    case "position":
                        rect.position = internalReader.Deserialize<Vector2Int>(reader);
                        break;
                    case "size":
                        rect.size = internalReader.Deserialize<Vector2Int>(reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
                return rect;
            }
        }

    }

}