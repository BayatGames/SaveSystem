using System;

using Bayat.Core;
using Bayat.Json.Serialization;

namespace Bayat.Json.Converters
{

    public class TransformConverter : ObjectJsonConverter
    {

        public override string[] GetObjectProperties()
        {
            return new string[] { "position", "rotation", "localScale", "parent", "hideFlags" };
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Transform);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            var instance = (UnityEngine.Transform)value;

            // We just store the reference instead of the whole transform
            //internalWriter.SerializeProperty(writer, "parent", instance.parent);
            if (SceneReferenceResolver.Current != null)
            {
                var unityGuid = SceneReferenceResolver.Current.Get(instance.parent);
                writer.WriteProperty("parentRef", unityGuid);
            }
            internalWriter.SerializeProperty(writer, "position", instance.position);
            internalWriter.SerializeProperty(writer, "rotation", instance.rotation);
            internalWriter.SerializeProperty(writer, "localScale", instance.localScale);
            internalWriter.SerializeProperty(writer, "hideFlags", instance.hideFlags);
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            var instance = (UnityEngine.Transform)targetObject;
            switch (memberName)
            {
                case "parent":

                    // Handles old data
                    instance.parent = internalReader.DeserializeProperty<UnityEngine.Transform>(reader);
                    break;
                case "parentRef":
                    string unityGuid = reader.ReadAsString();
                    if (SceneReferenceResolver.Current != null)
                    {
                        var parent = (UnityEngine.Transform)SceneReferenceResolver.Current.Get(unityGuid);
                        if (parent != null)
                        {
                            instance.parent = parent;
                        }
                    }
                    break;
                case "position":
                    instance.position = internalReader.DeserializeProperty<UnityEngine.Vector3>(reader);
                    break;
                case "rotation":
                    instance.rotation = internalReader.DeserializeProperty<UnityEngine.Quaternion>(reader);
                    break;
                case "localScale":
                    instance.localScale = internalReader.DeserializeProperty<UnityEngine.Vector3>(reader);
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