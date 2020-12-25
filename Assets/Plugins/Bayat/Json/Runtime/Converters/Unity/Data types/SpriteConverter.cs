using Bayat.Json.Serialization;
using System;
using UnityEngine;

namespace Bayat.Json.Converters
{

    public class SpriteConverter : ObjectJsonConverter
    {

        public override object Create(JsonReader reader, JsonSerializerReader internalReader, JsonObjectContract objectContract, string id, string unityGuid, Type objectType, out bool exit)
        {
            Texture2D texture = internalReader.DeserializeProperty<UnityEngine.Texture2D>(reader);
            Rect textureRect = internalReader.DeserializeProperty<Rect>(reader);
            Vector2 pivot = internalReader.DeserializeProperty<UnityEngine.Vector2>(reader);
            float pixelsPerUnit = reader.ReadProperty<float>();
            Vector4 border = internalReader.DeserializeProperty<Vector4>(reader);
            exit = false;
            return Sprite.Create(texture, textureRect, pivot, pixelsPerUnit, 0, SpriteMeshType.Tight, border);
        }

        public override string[] GetObjectProperties()
        {
            return new string[] { "name", "texture", "textureRect", "pivot", "pixelsPerUnit", "border", "hideFlags" };
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Sprite);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            var instance = (UnityEngine.Sprite)value;
            internalWriter.SerializeProperty(writer, "texture", instance.texture);
            internalWriter.SerializeProperty(writer, "textureRect", instance.textureRect);
            internalWriter.SerializeProperty(writer, "pivot", instance.pivot);
            writer.WriteProperty("pixelsPerUnit", instance.pixelsPerUnit);
            internalWriter.SerializeProperty(writer, "border", instance.border);
            writer.WriteProperty("name", instance.name);
            internalWriter.SerializeProperty(writer, "hideFlags", instance.hideFlags);
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            var instance = (UnityEngine.Sprite)targetObject;
            switch (memberName)
            {
                case "name":
                    instance.name = reader.ReadProperty<System.String>();
                    break;
                case "hideFlags":
                    instance.hideFlags = internalReader.DeserializeProperty<UnityEngine.HideFlags>(reader);
                    break;
                default:
                    reader.Skip();
                    break;
            }
            return instance;
        }

    }

}