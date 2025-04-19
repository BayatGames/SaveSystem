using System;

using Bayat.Json.Serialization;

namespace Bayat.Json.Converters
{

    public class MeshFilterConverter : UnityComponentConverter
    {

        public override string[] GetObjectProperties()
        {
            return new string[] { "sharedMesh", "hideFlags" };
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.MeshFilter);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            base.WriteProperties(contract, writer, value, objectType, internalWriter);

            var instance = (UnityEngine.MeshFilter)value;
            internalWriter.SerializeProperty(writer, "sharedMesh", instance.sharedMesh);
            internalWriter.SerializeProperty(writer, "hideFlags", instance.hideFlags);
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            var instance = (UnityEngine.MeshFilter)targetObject;
            switch (memberName)
            {
                case "sharedMesh":
                    instance.sharedMesh = internalReader.DeserializeProperty<UnityEngine.Mesh>(reader);
                    break;
                case "hideFlags":
                    instance.hideFlags = internalReader.DeserializeProperty<UnityEngine.HideFlags>(reader);
                    break;
                default:
                    base.PopulateMember(memberName, contract, reader, objectType, targetObject, internalReader);
                    break;
            }

            return instance;
        }

    }

}