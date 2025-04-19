using System;

using Bayat.Json.Serialization;

namespace Bayat.Json.Converters
{

    public class RectTransformConverter : UnityComponentConverter
    {

        public override string[] GetObjectProperties()
        {
            return new string[] { "anchorMin", "anchorMax", "anchoredPosition", "sizeDelta", "pivot", "offsetMin", "offsetMax", "position", "rotation", "localScale", "parent" };
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.RectTransform);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            base.WriteProperties(contract, writer, value, objectType, internalWriter);

            var instance = (UnityEngine.RectTransform)value;
            internalWriter.SerializeProperty(writer, "anchorMin", instance.anchorMin);
            internalWriter.SerializeProperty(writer, "anchorMax", instance.anchorMax);
            internalWriter.SerializeProperty(writer, "anchoredPosition", instance.anchoredPosition);
            internalWriter.SerializeProperty(writer, "sizeDelta", instance.sizeDelta);
            internalWriter.SerializeProperty(writer, "pivot", instance.pivot);
            internalWriter.SerializeProperty(writer, "offsetMin", instance.offsetMin);
            internalWriter.SerializeProperty(writer, "offsetMax", instance.offsetMax);
            internalWriter.SerializeProperty(writer, "position", instance.position);
            internalWriter.SerializeProperty(writer, "rotation", instance.rotation);
            internalWriter.SerializeProperty(writer, "localScale", instance.localScale);
            internalWriter.SerializeProperty(writer, "parent", instance.parent);
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            var instance = (UnityEngine.RectTransform)targetObject;
            switch (memberName)
            {
                case "anchorMin":
                    instance.anchorMin = internalReader.DeserializeProperty<UnityEngine.Vector2>(reader);
                    break;
                case "anchorMax":
                    instance.anchorMax = internalReader.DeserializeProperty<UnityEngine.Vector2>(reader);
                    break;
                case "anchoredPosition":
                    instance.anchoredPosition = internalReader.DeserializeProperty<UnityEngine.Vector2>(reader);
                    break;
                case "sizeDelta":
                    instance.sizeDelta = internalReader.DeserializeProperty<UnityEngine.Vector2>(reader);
                    break;
                case "pivot":
                    instance.pivot = internalReader.DeserializeProperty<UnityEngine.Vector2>(reader);
                    break;
                case "offsetMin":
                    instance.offsetMin = internalReader.DeserializeProperty<UnityEngine.Vector2>(reader);
                    break;
                case "offsetMax":
                    instance.offsetMax = internalReader.DeserializeProperty<UnityEngine.Vector2>(reader);
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
                case "parent":
                    instance.SetParent(internalReader.DeserializeProperty<UnityEngine.Transform>(reader), false);
                    break;
                default:
                    base.PopulateMember(memberName, contract, reader, objectType, targetObject, internalReader);
                    break;
            }

            return instance;
        }

    }

}