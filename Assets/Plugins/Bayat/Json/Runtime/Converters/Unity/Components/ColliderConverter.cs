using Bayat.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Bayat.Json.Converters
{

    public class ColliderConverter : UnityComponentConverter
    {

        public override bool IsGenericConverter
        {
            get
            {
                return true;
            }
        }

        protected List<string> baseProperties;
        protected List<string> propertiesToIgnore;

        public ColliderConverter()
        {
            this.baseProperties = base.GetSerializedProperties();
            this.propertiesToIgnore = new List<string>(this.baseProperties);
            this.propertiesToIgnore.Add("material");
        }

        public override string[] GetObjectProperties()
        {
            return new string[] { "isTrigger", "contactOffset", "sharedMaterial" };
        }

        public override List<string> GetSerializedProperties()
        {
            var list = new List<string>(base.GetSerializedProperties());
            list.AddRange(GetObjectProperties());
            return list;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(UnityEngine.Collider).IsAssignableFrom(objectType);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            var instance = (UnityEngine.Collider)value;
            writer.WriteProperty("isTrigger", instance.isTrigger);
            writer.WriteProperty("contactOffset", instance.contactOffset);
            internalWriter.SerializeProperty(writer, "sharedMaterial", instance.sharedMaterial);
            internalWriter.SerializeObjectProperties(writer, value, contract, null, null, null, this.propertiesToIgnore.ToArray());
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            var instance = (UnityEngine.Collider)targetObject;
            switch (memberName)
            {
                case "isTrigger":
                    instance.isTrigger = reader.ReadProperty<System.Boolean>();
                    break;
                case "contactOffset":
                    instance.contactOffset = reader.ReadProperty<System.Single>();
                    break;
                case "sharedMaterial":
                    #if UNITY_6000_0_OR_NEWER
                    instance.sharedMaterial = internalReader.DeserializeProperty<UnityEngine.PhysicsMaterial>(reader);
                    #else
                    instance.sharedMaterial = internalReader.DeserializeProperty<UnityEngine.PhysicMaterial>(reader);
                    #endif
                    break;
                default:
                    base.PopulateMember(memberName, contract, reader, objectType, targetObject, internalReader);
                    break;
            }
            return instance;
        }

    }

}