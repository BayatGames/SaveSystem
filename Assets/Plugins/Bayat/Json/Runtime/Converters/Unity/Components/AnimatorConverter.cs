using System;

using Bayat.Json.Serialization;

namespace Bayat.Json.Converters
{

    public class AnimatorConverter : ObjectJsonConverter
    {

        public override string[] GetObjectProperties()
        {
            return new string[] { "position", "rotation", "localScale", "parent", "hideFlags" };
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Animator);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            var instance = (UnityEngine.Animator)value;
            internalWriter.SerializeProperty(writer, "rootPosition", instance.rootPosition);
            internalWriter.SerializeProperty(writer, "rootRotation", instance.rootRotation);
            internalWriter.SerializeProperty(writer, "applyRootMotion", instance.applyRootMotion);
#if !UNITY_2018_3_OR_NEWER
            internalWriter.SerializeProperty(writer, "linearVelocityBlending", instance.linearVelocityBlending);
#endif
            internalWriter.SerializeProperty(writer, "updateMode", instance.updateMode);
            internalWriter.SerializeProperty(writer, "bodyPosition", instance.bodyPosition);
            internalWriter.SerializeProperty(writer, "bodyRotation", instance.bodyRotation);
            internalWriter.SerializeProperty(writer, "stabilizeFeet", instance.stabilizeFeet);
            internalWriter.SerializeProperty(writer, "feetPivotActive", instance.feetPivotActive);
            internalWriter.SerializeProperty(writer, "speed", instance.speed);
            internalWriter.SerializeProperty(writer, "cullingMode", instance.cullingMode);
            internalWriter.SerializeProperty(writer, "playbackTime", instance.playbackTime);
            internalWriter.SerializeProperty(writer, "recorderStartTime", instance.recorderStartTime);
            internalWriter.SerializeProperty(writer, "recorderStopTime", instance.recorderStopTime);
            internalWriter.SerializeProperty(writer, "runtimeAnimatorController", instance.runtimeAnimatorController);
            internalWriter.SerializeProperty(writer, "avatar", instance.avatar);
            internalWriter.SerializeProperty(writer, "layersAffectMassCenter", instance.layersAffectMassCenter);
            internalWriter.SerializeProperty(writer, "logWarnings", instance.logWarnings);
            internalWriter.SerializeProperty(writer, "fireEvents", instance.fireEvents);
            internalWriter.SerializeProperty(writer, "enabled", instance.enabled);
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            var instance = (UnityEngine.Animator)targetObject;
            switch (memberName)
            {
                case "rootPosition":
                    instance.rootPosition = internalReader.DeserializeProperty<UnityEngine.Vector3>(reader);
                    break;
                case "rootRotation":
                    instance.rootRotation = internalReader.DeserializeProperty<UnityEngine.Quaternion>(reader);
                    break;
                case "applyRootMotion":
                    instance.applyRootMotion = internalReader.DeserializeProperty<bool>(reader);
                    break;
#if !UNITY_2018_3_OR_NEWER
                case "linearVelocityBlending":
                    instance.linearVelocityBlending = internalReader.DeserializeProperty<bool>(reader);
                    break;
#endif
                case "updateMode":
                    instance.updateMode = internalReader.DeserializeProperty<UnityEngine.AnimatorUpdateMode>(reader);
                    break;
                case "bodyPosition":
                    instance.bodyPosition = internalReader.DeserializeProperty<UnityEngine.Vector3>(reader);
                    break;
                case "bodyRotation":
                    instance.bodyRotation = internalReader.DeserializeProperty<UnityEngine.Quaternion>(reader);
                    break;
                case "stabilizeFeet":
                    instance.stabilizeFeet = internalReader.DeserializeProperty<bool>(reader);
                    break;
                case "feetPivotActive":
                    instance.feetPivotActive = internalReader.DeserializeProperty<float>(reader);
                    break;
                case "speed":
                    instance.speed = internalReader.DeserializeProperty<float>(reader);
                    break;
                case "cullingMode":
                    instance.cullingMode = internalReader.DeserializeProperty<UnityEngine.AnimatorCullingMode>(reader);
                    break;
                case "playbackTime":
                    instance.playbackTime = internalReader.DeserializeProperty<float>(reader);
                    break;
                case "recorderStartTime":
                    instance.recorderStartTime = internalReader.DeserializeProperty<float>(reader);
                    break;
                case "recorderStopTime":
                    instance.recorderStopTime = internalReader.DeserializeProperty<float>(reader);
                    break;
                case "runtimeAnimatorController":
                    instance.runtimeAnimatorController = internalReader.DeserializeProperty<UnityEngine.RuntimeAnimatorController>(reader);
                    break;
                case "avatar":
                    instance.avatar = internalReader.DeserializeProperty<UnityEngine.Avatar>(reader);
                    break;
                case "layersAffectMassCenter":
                    instance.layersAffectMassCenter = internalReader.DeserializeProperty<bool>(reader);
                    break;
                case "logWarnings":
                    instance.logWarnings = internalReader.DeserializeProperty<bool>(reader);
                    break;
                case "fireEvents":
                    instance.fireEvents = internalReader.DeserializeProperty<bool>(reader);
                    break;
                case "enabled":
                    instance.enabled = internalReader.DeserializeProperty<bool>(reader);
                    break;
                default:
                    reader.Skip();
                    break;
            }
            return instance;
        }

    }

}