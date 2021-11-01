using System;
using System.Collections.Generic;
using System.Globalization;

using Bayat.Core;
using Bayat.Json.Linq;
using Bayat.Json.Serialization;
using Bayat.Json.Utilities;

namespace Bayat.Json.Converters
{

    public abstract class ObjectJsonConverter : JsonConverter
    {

        public virtual string[] GetObjectProperties()
        {
            return new string[0];
        }

        public virtual List<string> GetSerializedProperties()
        {
            return new List<string>();
        }

        public virtual object Create(JsonReader reader, JsonSerializerReader internalReader, JsonObjectContract objectContract, string id, string unityGuid, Type objectType, out bool exit)
        {
            return internalReader.CreateNewObject(reader, objectContract, null, null, id, unityGuid, out exit);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializerWriter internalWriter)
        {
            if (value == null || (value is UnityEngine.Object && value as UnityEngine.Object == null))
            {
                writer.WriteNull();
                return;
            }
            Type objectType = value.GetType();
            internalWriter._rootType = objectType;
            internalWriter._rootLevel = internalWriter._serializeStack.Count + 1;
            JsonObjectContract contract = (JsonObjectContract)internalWriter.GetContractSafe(value);

            try
            {
                if (internalWriter.ShouldWriteReference(value, null, contract, null, null))
                {
                    internalWriter.WriteReference(writer, value);
                }
                else
                {
                    if (value == null)
                    {
                        writer.WriteNull();
                        return;
                    }
                    internalWriter.OnSerializing(writer, contract, value);

                    internalWriter._serializeStack.Add(value);

                    internalWriter.WriteObjectStart(writer, value, contract, null, null, null);

                    WriteProperties(contract, writer, value, objectType, internalWriter);

                    writer.WriteEndObject();

                    internalWriter._serializeStack.RemoveAt(internalWriter._serializeStack.Count - 1);

                    internalWriter.OnSerialized(writer, contract, value);
                }
            }
            catch (Exception ex)
            {
                if (internalWriter.IsErrorHandled(null, contract, null, null, writer.Path, ex))
                {
                    internalWriter.HandleError(writer, 0);
                }
                else
                {
                    // clear context in case serializer is being used inside a converter
                    // if the converter wraps the error then not clearing the context will cause this error:
                    // "Current error context error is different to requested error."
                    internalWriter.ClearErrorContext();
                    throw;
                }
            }
            finally
            {
                // clear root contract to ensure that if level was > 1 then it won't
                // accidently be used for non root values
                internalWriter._rootType = null;
            }
        }

        public abstract void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializerReader internalReader)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (!ReflectionUtils.IsNullable(objectType))
                {
                    throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
                }

                return null;
            }
            if (objectType == null && existingValue != null)
            {
                objectType = existingValue.GetType();
            }

            JsonContract contract = internalReader.GetContractSafe(objectType);
            UnityEngine.Object unityObject = null;

            if (!reader.MoveToContent())
            {
                throw JsonSerializationException.Create(reader, "No JSON content found.");
            }
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                string id;
                string unityGuid;
                Type resolvedObjectType = objectType;

                if (internalReader.Serializer.MetadataPropertyHandling == MetadataPropertyHandling.Ignore)
                {
                    // don't look for metadata properties
                    reader.ReadAndAssert();
                    id = null;
                    unityGuid = null;
                }
                else if (internalReader.Serializer.MetadataPropertyHandling == MetadataPropertyHandling.ReadAhead)
                {
                    JTokenReader tokenReader = reader as JTokenReader;
                    if (tokenReader == null)
                    {
                        JToken t = JToken.ReadFrom(reader);
                        tokenReader = (JTokenReader)t.CreateReader();
                        tokenReader.Culture = reader.Culture;
                        tokenReader.DateFormatString = reader.DateFormatString;
                        tokenReader.DateParseHandling = reader.DateParseHandling;
                        tokenReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
                        tokenReader.FloatParseHandling = reader.FloatParseHandling;
                        tokenReader.SupportMultipleContent = reader.SupportMultipleContent;

                        // start
                        tokenReader.ReadAndAssert();

                        reader = tokenReader;
                    }

                    object newValue;
                    if (internalReader.ReadMetadataPropertiesToken(tokenReader, ref resolvedObjectType, ref contract, null, null, null, existingValue, out newValue, out id, out unityGuid, out unityObject))
                    {
                        if (SceneReferenceResolver.Current != null && !string.IsNullOrEmpty(unityGuid) && !AssetReferenceResolver.Current.Contains(unityGuid))
                        {
                            SceneReferenceResolver.Current.Add(unityObject, unityGuid);
                            //if (SceneReferenceResolver.Current.Contains(unityGuid))
                            //{
                            //    SceneReferenceResolver.Current.Set(unityGuid, unityObject);
                            //}
                            //else
                            //{
                            //    SceneReferenceResolver.Current.Add(unityGuid, unityObject);
                            //}
                        }
                        if (unityObject != null)
                        {
                            return unityObject;
                        }
                        return newValue;
                    }
                }
                else
                {
                    reader.ReadAndAssert();
                    object newValue;
                    if (internalReader.ReadMetadataProperties(reader, ref resolvedObjectType, ref contract, null, null, null, existingValue, out newValue, out id, out unityGuid, out unityObject))
                    {
                        if (SceneReferenceResolver.Current != null && !string.IsNullOrEmpty(unityGuid) && !AssetReferenceResolver.Current.Contains(unityGuid))
                        {
                            SceneReferenceResolver.Current.Add(unityObject, unityGuid);
                            //if (SceneReferenceResolver.Current.Contains(unityGuid))
                            //{
                            //    SceneReferenceResolver.Current.Set(unityGuid, unityObject);
                            //}
                            //else
                            //{
                            //    SceneReferenceResolver.Current.Add(unityGuid, unityObject);
                            //}
                        }
                        if (unityObject != null)
                        {
                            return unityObject;
                        }
                        return newValue;
                    }
                }

                if (internalReader.HasNoDefinedType(contract))
                {
                    return internalReader.CreateJObject(reader);
                }

                bool createdFromNonDefaultCreator = false;
                JsonObjectContract objectContract = (JsonObjectContract)contract;
                object targetObject;

                // check that if type name handling is being used that the existing value is compatible with the specified type
                if (existingValue != null && (resolvedObjectType == objectType || resolvedObjectType.IsAssignableFrom(existingValue.GetType())))
                {
                    targetObject = existingValue;
                }
                else if (unityObject != null)
                {
                    targetObject = unityObject;
                }
                else
                {
                    targetObject = Create(reader, internalReader, objectContract, id, unityGuid, objectType, out createdFromNonDefaultCreator);
                }

                if (SceneReferenceResolver.Current != null && !string.IsNullOrEmpty(unityGuid) && !AssetReferenceResolver.Current.Contains(unityGuid))
                {
                    SceneReferenceResolver.Current.Add((UnityEngine.Object)targetObject, unityGuid);
                    //if (SceneReferenceResolver.Current.Contains(unityGuid))
                    //{
                    //    SceneReferenceResolver.Current.Set(unityGuid, (UnityEngine.Object)targetObject);
                    //}
                    //else
                    //{
                    //    SceneReferenceResolver.Current.Add(unityGuid, (UnityEngine.Object)targetObject);
                    //}
                }

                // don't populate if read from non-default creator because the object has already been read
                if (createdFromNonDefaultCreator)
                {
                    return targetObject;
                }
                internalReader.OnDeserializing(reader, contract, targetObject);
                bool referenceAdded = false;
                if (id != null && targetObject != null)
                {
                    internalReader.AddReference(reader, id, targetObject);
                    referenceAdded = true;
                }
                targetObject = Populate(contract, reader, objectType, targetObject, internalReader);
                if (id != null && targetObject != null && !referenceAdded)
                {
                    internalReader.AddReference(reader, id, targetObject);
                }
                internalReader.OnDeserialized(reader, contract, targetObject);
                return targetObject;
            }
            else
            {
                throw JsonSerializationException.Create(reader, "Unexpected initial token '{0}' when populating object. Expected JSON object.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
        }

        public virtual object Populate(JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            //JObject obj = (JObject)internalReader.CreateJObject(reader);
            //return PopulateJObject(obj, contract, reader, objectType, targetObject, internalReader);
            foreach (string propertyName in GetProperties(reader, internalReader))
            {
                //reader.ReadAndMoveToContent();
                targetObject = PopulateMember(propertyName, contract, reader, objectType, targetObject, internalReader);
            }
            return targetObject;
        }

        public virtual IEnumerable<string> GetProperties(JsonReader reader, JsonSerializerReader internalReader)
        {
            if (reader.TokenType != JsonToken.PropertyName)
            {
                reader.ReadAndMoveToContent();
            }
            bool finished = false;
            do
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string memberName = reader.Value.ToString();
                        if (internalReader.CheckPropertyName(reader, memberName))
                        {
                            continue;
                        }
                        yield return memberName;
                        break;
                    case JsonToken.EndObject:
                        finished = true;
                        break;
                    case JsonToken.Comment:
                        // ignore
                        break;
                    default:
                        throw JsonSerializationException.Create(reader, "Unexpected token when deserializing object: " + reader.TokenType);
                }
            } while (!finished && reader.Read());
        }

        //public virtual object PopulateJObject(JObject obj, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        //{
        //    throw new NotImplementedException();
        //}

        public virtual object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            object populatedObject = internalReader.PopulateObjectProperty(targetObject, reader, null, memberName, (JsonObjectContract)contract);
            if (populatedObject == null)
            {
                return targetObject;
            }
            return populatedObject;
        }

    }

}