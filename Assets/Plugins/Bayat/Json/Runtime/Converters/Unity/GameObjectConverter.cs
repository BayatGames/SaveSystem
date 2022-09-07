using System;
using System.Collections.Generic;

using Bayat.Json.Serialization;

using UnityEngine;

namespace Bayat.Json.Converters
{

    public class GameObjectConverter : UnityObjectConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return typeof(UnityEngine.GameObject).IsAssignableFrom(objectType) && base.CanConvert(objectType);
        }

        public GameObjectConverter()
        {
        }

        public override string[] GetObjectProperties()
        {
            return new string[] { "name", "tag", "active", "isStatic", "layer", "hideFlags", "components", "children" };
        }

        public override List<string> GetSerializedProperties()
        {
            var list = new List<string>(base.GetSerializedProperties());
            list.AddRange(GetObjectProperties());
            return list;
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            base.WriteProperties(contract, writer, value, objectType, internalWriter);

            GameObject gameObject = (GameObject)value;
            GameObjectSerializationHandler serializationHandler = gameObject.GetComponent<GameObjectSerializationHandler>();
            writer.WriteProperty("name", gameObject.name);
            writer.WriteProperty("tag", gameObject.tag);
            writer.WriteProperty("active", gameObject.activeSelf);
            writer.WriteProperty("isStatic", gameObject.isStatic);
            writer.WriteProperty("layer", gameObject.layer);
            writer.WriteProperty("hideFlags", gameObject.hideFlags);

            bool serializeChildren = true;
            if (serializationHandler != null)
            {
                serializeChildren = serializationHandler.SerializeChildren;
            }
            if (serializeChildren)
            {
                writer.WritePropertyName("children");
                writer.WriteStartArray();
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    Transform child = gameObject.transform.GetChild(i);
                    bool shouldSerialize = true;
                    if (serializationHandler != null)
                    {
                        shouldSerialize = serializationHandler.ShouldSerializeChild(child);
                    }
                    if (shouldSerialize)
                    {
                        internalWriter.Serialize(writer, child.gameObject);
                    }
                }
                writer.WriteEndArray();
            }

            bool serializeComponents = true;
            if (serializationHandler != null)
            {
                serializeComponents = serializationHandler.SerializeComponents;
            }
            if (serializeComponents)
            {
                writer.WritePropertyName("components");
                writer.WriteStartArray();
                Component[] components = gameObject.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    Component component = components[i];
                    bool shouldSerialize = true;
                    if (serializationHandler != null)
                    {
                        shouldSerialize = serializationHandler.ShouldSerializeComponent(component);
                    }
                    if (shouldSerialize)
                    {
                        writer.WriteStartObject();
                        internalWriter.WriteTypeProperty(writer, component.GetType());
                        writer.WritePropertyName(JsonTypeReflector.ValuePropertyName);
                        internalWriter.Serialize(writer, component);
                        writer.WriteEndObject();
                    }
                }
                writer.WriteEndArray();
            }
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            GameObject gameObject = (GameObject)targetObject;
            bool finished;
            switch (memberName)
            {
                case "name":
                    gameObject.name = reader.ReadAsString();
                    break;
                case "tag":
                    gameObject.tag = reader.ReadAsString();
                    break;
                case "active":
                    gameObject.SetActive(reader.ReadAsBoolean().GetValueOrDefault());
                    break;
                case "isStatic":
                    gameObject.isStatic = reader.ReadAsBoolean().GetValueOrDefault();
                    break;
                case "layer":
                    gameObject.layer = reader.ReadAsInt32().GetValueOrDefault();
                    break;
                case "hideFlags":
                    gameObject.hideFlags = (HideFlags)reader.ReadAsInt32().GetValueOrDefault();
                    break;
                case "children":

                    // Skip property name
                    reader.ReadAndAssert();

                    // Skipy array start
                    reader.ReadAndAssert();

                    finished = false;
                    do
                    {
                        switch (reader.TokenType)
                        {
                            case JsonToken.EndArray:
                                finished = true;
                                break;
                            default:
                                if (reader.TokenType != JsonToken.StartObject)
                                {
                                    reader.Read();
                                    continue;
                                }
                                GameObject child = internalReader.Deserialize<GameObject>(reader);
                                child.transform.SetParent(gameObject.transform);
                                //child.transform.parent = gameObject.transform;
                                break;
                        }
                    } while (!finished);
                    break;
                case "components":

                    // Skip property name
                    reader.ReadAndAssert();

                    // Skipy array start
                    reader.ReadAndAssert();

                    finished = false;
                    do
                    {
                        switch (reader.TokenType)
                        {
                            case JsonToken.EndArray:
                                finished = true;
                                break;
                            default:
                                if (reader.TokenType != JsonToken.StartObject)
                                {
                                    reader.Read();
                                    continue;
                                }
                                reader.ReadAndAssert();
                                string qualifiedTypeName = reader.ReadAsString();
                                Type componentType = null;
                                JsonContract componentContract = null;
                                internalReader.ResolveTypeName(reader, ref componentType, ref componentContract, null, null, null, qualifiedTypeName);
                                reader.ReadAndAssert();
                                reader.ReadAndAssert();
                                Component component = gameObject.GetComponent(componentType);
                                if (component == null)
                                {
                                    component = gameObject.AddComponent(componentType);
                                }
                                internalReader.DeserializeInto(reader, componentType, component, false);
                                break;
                        }
                    } while (!finished);
                    break;
                default:
                    base.PopulateMember(memberName, contract, reader, objectType, targetObject, internalReader);
                    break;
            }
            return gameObject;
        }

    }

}