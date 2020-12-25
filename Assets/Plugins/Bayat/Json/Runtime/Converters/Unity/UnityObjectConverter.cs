using Bayat.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Bayat.Json.Converters
{

    public abstract class UnityObjectConverter : ObjectJsonConverter
    {

        public override bool IsGenericConverter
        {
            get
            {
                return true;
            }
        }

        public UnityObjectConverter()
        {
        }

        public override string[] GetObjectProperties()
        {
            return new string[] { "instanceID" };
        }

        public override List<string> GetSerializedProperties()
        {
            var list = new List<string>(base.GetSerializedProperties());
            list.AddRange(GetObjectProperties());
            return list;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(UnityEngine.Object).IsAssignableFrom(objectType);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            var instance = (UnityEngine.Object)value;
            writer.WriteProperty("instanceID", instance.GetInstanceID());
        }

    }

}