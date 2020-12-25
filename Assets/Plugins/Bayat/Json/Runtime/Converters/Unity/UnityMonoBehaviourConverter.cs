using Bayat.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Bayat.Json.Converters
{

    public class UnityMonoBehaviourConverter : UnityBehaviourConverter
    {

        public override bool IsGenericConverter
        {
            get
            {
                return true;
            }
        }

        protected List<string> baseProperties;

        public UnityMonoBehaviourConverter()
        {
            this.baseProperties = base.GetSerializedProperties();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(UnityEngine.MonoBehaviour).IsAssignableFrom(objectType) && base.CanConvert(objectType);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            base.WriteProperties(contract, writer, value, objectType, internalWriter);
            internalWriter.SerializeObjectProperties(writer, value, contract, null, null, null, this.baseProperties.ToArray());
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            if (this.baseProperties.Contains(memberName))
            {
                return base.PopulateMember(memberName, contract, reader, objectType, targetObject, internalReader);
            }
            object populatedObject = internalReader.PopulateObjectProperty(targetObject, reader, null, memberName, (JsonObjectContract)contract);
            if (populatedObject == null)
            {
                return targetObject;
            }
            return populatedObject;
        }

    }

}