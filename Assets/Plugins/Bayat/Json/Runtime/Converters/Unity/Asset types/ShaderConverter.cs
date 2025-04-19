using System;

using Bayat.Json.Serialization;

using UnityEngine;

namespace Bayat.Json.Converters
{

    public class ShaderConverter : UnityObjectConverter
    {

        public override object Create(JsonReader reader, JsonSerializerReader internalReader, JsonObjectContract objectContract, string id, string unityGuid, string gameObjectGuid, Type objectType, out bool exit)
        {
            exit = false;
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Shader);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            Shader shader = (Shader)value;
            writer.WriteProperty("name", shader.name);
            writer.WriteProperty("maximumLOD", shader.maximumLOD);
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            Shader shader = (Shader)targetObject;
            switch (memberName)
            {
                case "name":
                    string name = reader.ReadProperty<string>();
                    if (shader == null)
                    {
                        shader = Shader.Find(name);
                    }
                    else
                    {
                        shader.name = name;
                    }
                    if (shader == null)
                    {
                        shader = Shader.Find("Diffuse");
                    }
                    break;
                case "maximumLOD":
                    shader.maximumLOD = reader.ReadProperty<int>();
                    break;
                default:
                    reader.Skip();
                    break;
            }
            return shader;
        }

    }

}