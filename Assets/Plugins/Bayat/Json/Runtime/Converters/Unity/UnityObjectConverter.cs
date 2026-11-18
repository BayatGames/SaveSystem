using System;
using System.Collections.Generic;

using Bayat.Json.Serialization;

namespace Bayat.Json.Converters
{

    public abstract class UnityObjectConverter : ObjectJsonConverter
    {

        private const string InstanceIdProperty = "instanceID";

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
            return new string[] { InstanceIdProperty };
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
#if UNITY_6000_4_OR_NEWER
            // This is legacy, session-local metadata. The loader resolves references using
            // Bayat's GUIDs and JSON reference IDs. Keep the Int32 field without truncating an EntityId.
            writer.WriteProperty(InstanceIdProperty, 0);
#else
            writer.WriteProperty(InstanceIdProperty, ((UnityEngine.Object)value).GetInstanceID());
#endif
        }

    }

}
