using Bayat.Json.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bayat.Json.Converters
{

    public class RendererConverter : UnityComponentConverter
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

        public RendererConverter()
        {
            this.baseProperties = base.GetSerializedProperties();
            this.propertiesToIgnore = new List<string>();
            this.propertiesToIgnore.Add("sharedMaterial");
            this.propertiesToIgnore.Add("material");
            this.propertiesToIgnore.Add("materials");
        }

        public override string[] GetObjectProperties()
        {
            return new string[] { "sharedMaterials" };
        }

        public override List<string> GetSerializedProperties()
        {
            var list = new List<string>(base.GetSerializedProperties());
            list.AddRange(GetObjectProperties());
            return list;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(UnityEngine.Renderer).IsAssignableFrom(objectType);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            //var instance = (UnityEngine.Renderer)value;
            //WriteObjectProperties(writer, instance, internalWriter);
            internalWriter.SerializeObjectProperties(writer, value, contract, null, null, null, this.propertiesToIgnore.ToArray());
        }

        public virtual void WriteObjectProperties(JsonWriter writer, Renderer instance, JsonSerializerWriter internalWriter)
        {
            internalWriter.SerializeProperty(writer, "sharedMaterials", instance.sharedMaterials);
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            var instance = (UnityEngine.Renderer)targetObject;
            switch (memberName)
            {
                case "sharedMaterials":
                    instance.sharedMaterials = internalReader.DeserializeProperty<UnityEngine.Material[]>(reader);
                    break;
                default:
                    base.PopulateMember(memberName, contract, reader, objectType, targetObject, internalReader);
                    break;
            }
            return instance;
        }

    }

}