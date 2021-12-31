using System;
using System.Collections.Generic;

using Bayat.Core;
using Bayat.Json.Serialization;

using UnityEngine;

namespace Bayat.Json.Converters
{

    public class MaterialConverter : UnityObjectConverter
    {

        [Serializable]
        public struct JsonSerializedMaterialProperty
        {
            public string name;
            public RuntimeMaterialPropertyType type;
            public object value;
        }

        public const string MaterialPropertyNamePrefix = "$materialProperty_";
        public const string MaterialPropertiesName = "$materialProperties";

        public override object Create(JsonReader reader, JsonSerializerReader internalReader, JsonObjectContract objectContract, string id, string unityGuid, Type objectType, out bool exit)
        {
            exit = false;
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Material);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            Material material = (Material)value;
            internalWriter.SerializeProperty(writer, "shader", material.shader);
            writer.WriteProperty("name", material.name);
            internalWriter.SerializeProperty(writer, "shaderKeywords", material.shaderKeywords);
            writer.WriteProperty("renderQueue", material.renderQueue);
            writer.WriteProperty("globalIlluminationFlags", material.globalIlluminationFlags);
            writer.WriteProperty("doubleSidedGI", material.doubleSidedGI);
            writer.WriteProperty("enableInstancing", material.enableInstancing);
            RuntimeMaterialProperties properties = MaterialPropertiesResolver.Current.GetMaterialProperties(material);
            if (properties != null)
            {
                List<JsonSerializedMaterialProperty> serializableProperties = new List<JsonSerializedMaterialProperty>();
                for (int i = 0; i < properties.Properties.Count; i++)
                {
                    RuntimeMaterialProperty property = properties.Properties[i];
                    JsonSerializedMaterialProperty serializedProperty = new JsonSerializedMaterialProperty();
                    serializedProperty.name = property.Name;
                    serializedProperty.type = property.Type;
                    switch (property.Type)
                    {
                        case RuntimeMaterialPropertyType.Color:
                            serializedProperty.value = material.GetColor(property.Name);
                            break;
                        case RuntimeMaterialPropertyType.Vector:
                            serializedProperty.value = material.GetVector(property.Name);
                            break;
                        case RuntimeMaterialPropertyType.Float:
                            serializedProperty.value = material.GetFloat(property.Name);
                            break;
                        case RuntimeMaterialPropertyType.Range:
                            serializedProperty.value = material.GetFloat(property.Name);
                            break;
                        case RuntimeMaterialPropertyType.Texture:
                            serializedProperty.value = material.GetTexture(property.Name);
                            break;
                    }
                    serializableProperties.Add(serializedProperty);
                    //internalWriter.SerializeProperty(writer, MaterialPropertyNamePrefix + property.Name, serializedProperty);
                }
                internalWriter.SerializeProperty(writer, MaterialPropertiesName, serializableProperties);
            }

            // Serialize main properties when there are no pre-defined properties available for this material
            else
            {
                internalWriter.SerializeProperty(writer, "color", material.color);
                internalWriter.SerializeProperty(writer, "mainTexture", material.mainTexture);
                internalWriter.SerializeProperty(writer, "mainTextureOffset", material.mainTextureOffset);
                internalWriter.SerializeProperty(writer, "mainTextureScale", material.mainTextureScale);
            }
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            Material material = (Material)targetObject;
            switch (memberName)
            {
                case "shader":
                    Shader shader = internalReader.DeserializeProperty<Shader>(reader);
                    if (material == null)
                    {
                        material = new Material(shader);
                    }
                    else
                    {
                        material.shader = shader;
                    }
                    break;
                case "name":
                    material.name = reader.ReadProperty<string>();
                    break;
                case "shaderKeywords":
                    material.shaderKeywords = internalReader.DeserializeProperty<string[]>(reader);
                    break;
                case "renderQueue":
                    material.renderQueue = reader.ReadProperty<int>();
                    break;
                case "globalIlluminationFlag":
                    material.globalIlluminationFlags = internalReader.DeserializeProperty<MaterialGlobalIlluminationFlags>(reader);
                    break;
                case "color":
                    material.color = internalReader.DeserializeProperty<Color>(reader);
                    break;
                case "doubleSidedGI":
                    material.doubleSidedGI = reader.ReadProperty<bool>();
                    break;
                case "enableInstancing":
                    material.enableInstancing = reader.ReadProperty<bool>();
                    break;
                case "mainTexture":
                    material.mainTexture = internalReader.DeserializeProperty<Texture2D>(reader);
                    break;
                case "mainTextureOffset":
                    material.mainTextureOffset = internalReader.DeserializeProperty<Vector2>(reader);
                    break;
                case "mainTextureScale":
                    material.mainTextureScale = internalReader.DeserializeProperty<Vector2>(reader);
                    break;
                case MaterialPropertiesName:
                    List<JsonSerializedMaterialProperty> serializedProperties = internalReader.DeserializeProperty<List<JsonSerializedMaterialProperty>>(reader);
                    foreach (JsonSerializedMaterialProperty serializedProperty in serializedProperties)
                    {
                        if (serializedProperty.value != null)
                        {
                            switch (serializedProperty.type)
                            {
                                case RuntimeMaterialPropertyType.Color:
                                    material.SetColor(serializedProperty.name, (Color)serializedProperty.value);
                                    break;
                                case RuntimeMaterialPropertyType.Vector:
                                    material.SetVector(serializedProperty.name, (Vector4)serializedProperty.value);
                                    break;
                                case RuntimeMaterialPropertyType.Float:
                                    material.SetFloat(serializedProperty.name, (float)Convert.ChangeType(serializedProperty.value, typeof(float)));
                                    break;
                                case RuntimeMaterialPropertyType.Range:
                                    material.SetFloat(serializedProperty.name, (float)Convert.ChangeType(serializedProperty.value, typeof(float)));
                                    break;
                                case RuntimeMaterialPropertyType.Texture:
                                    material.SetTexture(serializedProperty.name, (Texture)serializedProperty.value);
                                    break;
                            }
                        }
                    }
                    break;
                default:
                    reader.Skip();
                    break;
            }
            return material;
        }

    }

}