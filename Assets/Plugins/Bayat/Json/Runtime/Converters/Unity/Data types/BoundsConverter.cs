using Bayat.Json.Serialization;
using System;
using UnityEngine;

namespace Bayat.Json.Converters
{

    public class BoundsConverter : ObjectJsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Bounds)
#if UNITY_2017_2_OR_NEWER
                || objectType == typeof(BoundsInt)
#endif
                ;
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            if (objectType == typeof(Bounds))
            {
                Bounds bounds = (Bounds)value;
                internalWriter.SerializeProperty(writer, "center", bounds.center);
                internalWriter.SerializeProperty(writer, "size", bounds.size);
            }
            else if (objectType == typeof(BoundsInt))
            {
                BoundsInt bounds = (BoundsInt)value;
                internalWriter.SerializeProperty(writer, "position", bounds.position);
                internalWriter.SerializeProperty(writer, "size", bounds.size);
            }
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            if (objectType == typeof(Bounds))
            {
                Bounds bounds = (Bounds)targetObject;
                switch (memberName)
                {
                    case "center":
                        bounds.center = internalReader.DeserializeProperty<Vector3>(reader);
                        break;
                    case "size":
                        bounds.size = internalReader.DeserializeProperty<Vector3>(reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
                return bounds;
            }
            else
            {
                BoundsInt bounds = (BoundsInt)targetObject;
                switch (memberName)
                {
                    case "position":
                        bounds.position = internalReader.DeserializeProperty<Vector3Int>(reader);
                        break;
                    case "size":
                        bounds.size = internalReader.DeserializeProperty<Vector3Int>(reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
                return bounds;
            }
        }

    }

}