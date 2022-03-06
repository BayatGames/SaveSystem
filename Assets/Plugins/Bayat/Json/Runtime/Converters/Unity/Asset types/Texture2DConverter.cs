using System;

using Bayat.Json.Serialization;

using UnityEngine;

namespace Bayat.Json.Converters
{

    public class Texture2DConverter : UnityObjectConverter
    {

        public override object Create(JsonReader reader, JsonSerializerReader internalReader, JsonObjectContract objectContract, string id, string unityGuid, Type objectType, out bool exit)
        {
            exit = false;
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Texture2D);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            Texture2D instance = (Texture2D)value;
            writer.WriteProperty("width", instance.width);
            writer.WriteProperty("height", instance.height);
            internalWriter.SerializeProperty(writer, "format", instance.format);
            writer.WriteProperty("mipmapCount", instance.mipmapCount);
            writer.WriteProperty("mipMapBias", instance.mipMapBias);
            writer.WriteProperty("anisoLevel", instance.anisoLevel);
            internalWriter.SerializeProperty(writer, "filterMode", instance.filterMode);
            internalWriter.SerializeProperty(writer, "wrapMode", instance.wrapMode);
            writer.WriteProperty("rawTextureData", instance.GetRawTextureData());
        }

        public override object Populate(JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            Texture2D instance = (Texture2D)targetObject;
            if (instance == null)
            {
                int width = reader.ReadProperty<int>();
                int height = reader.ReadProperty<int>();
                TextureFormat format = internalReader.DeserializeProperty<TextureFormat>(reader);
                int mipmapCount = reader.ReadProperty<int>();
                instance = new Texture2D(width, height, format, mipmapCount > 1);
                reader.ReadAndAssert();
            }
            foreach (string propertyName in GetProperties(reader, internalReader))
            {
                switch (propertyName)
                {
                    case "anisoLevel":
                        instance.anisoLevel = reader.ReadProperty<int>();
                        break;
                    case "filterMode":
                        instance.filterMode = internalReader.DeserializeProperty<FilterMode>(reader);
                        break;
                    case "wrapMode":
                        instance.wrapMode = internalReader.DeserializeProperty<TextureWrapMode>(reader);
                        break;
                    case "mipMapBias":
                        instance.mipMapBias = reader.ReadProperty<float>();
                        break;
                    case "rawTextureData":
                        // LoadRawTextureData requires that the correct width, height, TextureFormat and mipMaps are set before being called.
                        // If an error occurs here, it's likely that we're using LoadInto to load into a Texture which differs in these values.
                        // In this case, LoadInto should be avoided and Load should be used instead.
                        instance.LoadRawTextureData(reader.ReadProperty<byte[]>());
                        instance.Apply();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            return targetObject;
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            Texture2D instance = (Texture2D)targetObject;
            if (instance == null)
            {
                int width = reader.ReadAsInt32().GetValueOrDefault();
                reader.Read();
                int height = reader.ReadAsInt32().GetValueOrDefault();
                reader.Read();
                TextureFormat format = internalReader.Deserialize<TextureFormat>(reader);
                reader.Read();
                int mipmapCount = reader.ReadAsInt32().GetValueOrDefault();
                instance = new Texture2D(width, height, format, mipmapCount > 1);
            }
            else
            {
                switch (memberName)
                {
                    case "filterMode":
                        instance.filterMode = internalReader.Deserialize<FilterMode>(reader);
                        break;
                    case "anisoLevel":
                        instance.anisoLevel = reader.ReadAsInt32().GetValueOrDefault();
                        break;
                    case "wrapMode":
                        instance.wrapMode = internalReader.Deserialize<TextureWrapMode>(reader);
                        break;
                    case "mipMapBias":
                        instance.mipMapBias = (float)reader.ReadAsDecimal();
                        break;
                    case "rawTextureData":
                        // LoadRawTextureData requires that the correct width, height, TextureFormat and mipMaps are set before being called.
                        // If an error occurs here, it's likely that we're using LoadInto to load into a Texture which differs in these values.
                        // In this case, LoadInto should be avoided and Load should be used instead.
                        instance.LoadRawTextureData(reader.ReadAsBytes());
                        instance.Apply();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            return instance;
        }

    }

}