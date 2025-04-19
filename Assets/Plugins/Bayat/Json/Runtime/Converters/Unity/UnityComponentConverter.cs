using System;

using Bayat.Json.Serialization;

namespace Bayat.Json.Converters
{

    public abstract class UnityComponentConverter : UnityObjectConverter
    {

        public override bool IsGenericConverter
        {
            get
            {
                return true;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(UnityEngine.Component).IsAssignableFrom(objectType) && base.CanConvert(objectType);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            base.WriteProperties(contract, writer, value, objectType, internalWriter);

            var instance = (UnityEngine.Component)value;
            internalWriter.SerializeProperty(writer, "gameObject", instance.gameObject);
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            UnityEngine.Component component = (UnityEngine.Component)targetObject;
            switch (memberName)
            {
                case "gameObject":
                    internalReader.DeserializeIntoProperty(reader, component.gameObject);
                    break;
                default:
                    base.PopulateMember(memberName, contract, reader, objectType, targetObject, internalReader);
                    break;
            }
            return component;
        }

    }

}