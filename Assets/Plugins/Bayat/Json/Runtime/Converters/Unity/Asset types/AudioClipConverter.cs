using Bayat.Json.Serialization;
using System;
using UnityEngine;

namespace Bayat.Json.Converters
{

    public class AudioClipConverter : ObjectJsonConverter
    {

        public override object Create(JsonReader reader, JsonSerializerReader internalReader, JsonObjectContract objectContract, string id, string unityGuid, Type objectType, out bool exit)
        {
            string name = reader.ReadProperty<string>();
            int lengthSamples = reader.ReadProperty<int>();
            int channels = reader.ReadProperty<int>();
            int frequency = reader.ReadProperty<int>();
            exit = false;
            return AudioClip.Create(name, lengthSamples, channels, frequency, false);
        }

        public override string[] GetObjectProperties()
        {
            return new string[] { "name", "hideFlags", "lengthSamples", "channels", "frequency", "sampleData" };
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.AudioClip);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            var instance = (UnityEngine.AudioClip)value;
            float[] samples = new float[instance.samples * instance.channels];
            instance.GetData(samples, 0);
            writer.WriteProperty("name", instance.name);
            writer.WriteProperty("lengthSamples", instance.samples);
            writer.WriteProperty("channels", instance.channels);
            writer.WriteProperty("frequency", instance.frequency);
            internalWriter.SerializeProperty(writer, "hideFlags", instance.hideFlags);
            internalWriter.SerializeProperty(writer, "sampleData", samples);
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            var instance = (UnityEngine.AudioClip)targetObject;
            switch (memberName)
            {
                case "name":
                    instance.name = reader.ReadProperty<System.String>();
                    break;
                case "hideFlags":
                    instance.hideFlags = internalReader.DeserializeProperty<UnityEngine.HideFlags>(reader);
                    break;
                case "sampleData":
                    instance.SetData(internalReader.DeserializeProperty<float[]>(reader), 0);
                    break;
                default:
                    reader.Skip();
                    break;
            }
            return instance;
        }

    }

}