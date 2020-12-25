using Bayat.Json;
using System;
using System.Globalization;
using System.IO;

namespace Bayat.SaveSystem
{

    /// <summary>
    /// The Save System <see cref="JsonSerializer"/> wrapper.
    /// </summary>
    public class SaveSystemJsonSerializer
    {

        protected JsonSerializer jsonSerializer;

        /// <summary>
        /// The internal json serializer.
        /// </summary>
        public virtual JsonSerializer JsonSerializer
        {
            get
            {
                return this.jsonSerializer;
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SaveSystemJsonSerializer"/> using the specified settings.
        /// </summary>
        /// <param name="settings">The json serializer settings</param>
        public SaveSystemJsonSerializer(JsonSerializerSettings settings) : this(JsonSerializer.Create(settings))
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="SaveSystemJsonSerializer"/> using the specified json serializer.
        /// </summary>
        /// <param name="jsonSerializer">The json serializer</param>
        public SaveSystemJsonSerializer(JsonSerializer jsonSerializer)
        {
            this.jsonSerializer = jsonSerializer;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SaveSystemJsonSerializer"/> with default configuration.
        /// </summary>
        /// <returns></returns>
        public static SaveSystemJsonSerializer CreateDefault()
        {
            return new SaveSystemJsonSerializer(JsonSerializerSettingsPreset.DefaultPreset.NewSettings);
        }

        #region Serialize Methods

        /// <summary>
        /// Serializes the value to json string.
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The serialized json</returns>
        public string Serialize(object value)
        {
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                Serialize(stringWriter, value);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// Serializes the value to the stream.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <param name="value">The value</param>
        public void Serialize(Stream stream, object value)
        {
            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
                Serialize(streamWriter, value);
            }
        }

        /// <summary>
        /// Serializes the value to the text writer.
        /// </summary>
        /// <param name="textWriter">The text writer</param>
        /// <param name="value">The value</param>
        public void Serialize(TextWriter textWriter, object value)
        {
            using (JsonTextWriter jsonWriter = new JsonTextWriter(textWriter))
            {
                Serialize(jsonWriter, value);
            }
        }

        /// <summary>
        /// Serializes the value to the json writer.
        /// </summary>
        /// <param name="jsonWriter">The json writer</param>
        /// <param name="value">The value</param>
        public void Serialize(JsonWriter jsonWriter, object value)
        {
            this.jsonSerializer.Serialize(jsonWriter, value);
        }

        #endregion

        #region Deserialize Methods

        /// <summary>
        /// Deserializes the object from json string.
        /// </summary>
        /// <param name="json">The json string</param>
        /// <param name="objectType">The object type</param>
        /// <returns>The deserialized object</returns>
        public object Deserialize(string json, Type objectType)
        {
            using (StringReader stringReader = new StringReader(json))
            {
                return Deserialize(stringReader, objectType);
            }
        }

        /// <summary>
        /// Deserializes the object from the stream.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <param name="objectType">The object type</param>
        /// <returns>The deserialized object</returns>
        public object Deserialize(Stream stream, Type objectType)
        {
            using (StreamReader streamReader = new StreamReader(stream))
            {
                return Deserialize(streamReader, objectType);
            }
        }

        /// <summary>
        /// Deserializes the object from the text reader.
        /// </summary>
        /// <param name="textReader">The text reader</param>
        /// <param name="objectType">The object type</param>
        /// <returns>The deserialized object</returns>
        public object Deserialize(TextReader textReader, Type objectType)
        {
            using (JsonTextReader jsonReader = new JsonTextReader(textReader))
            {
                return Deserialize(jsonReader, objectType);
            }
        }

        /// <summary>
        /// Deserializes the object from the json reader.
        /// </summary>
        /// <param name="jsonReader">The json reader</param>
        /// <param name="objectType">The object type</param>
        /// <returns>The deserialized object</returns>
        public object Deserialize(JsonReader jsonReader, Type objectType)
        {
            return jsonSerializer.Deserialize(jsonReader, objectType);
        }

        #endregion

        #region DeserializeInto Methods

        /// <summary>
        /// Deserializes the data into the object from the json reader.
        /// </summary>
        /// <param name="json">The json string</param>
        /// <param name="target">The target object to deserialize the data into</param>
        /// <returns>The populated target object</returns>
        public object DeserializeInto(string json, object target)
        {
            using (StringReader stringReader = new StringReader(json))
            {
                return DeserializeInto(stringReader, target);
            }
        }

        /// <summary>
        /// Deserializes the data into the object from the json reader.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <param name="target">The target object to deserialize the data into</param>
        /// <returns>The populated target object</returns>
        public object DeserializeInto(Stream stream, object target)
        {
            using (StreamReader streamReader = new StreamReader(stream))
            {
                return DeserializeInto(streamReader, target);
            }
        }

        /// <summary>
        /// Deserializes the data into the object from the json reader.
        /// </summary>
        /// <param name="textReader">The text reader</param>
        /// <param name="target">The target object to deserialize the data into</param>
        /// <returns>The populated target object</returns>
        public object DeserializeInto(TextReader textReader, object target)
        {
            using (JsonTextReader jsonReader = new JsonTextReader(textReader))
            {
                return DeserializeInto(jsonReader, target);
            }
        }

        /// <summary>
        /// Deserializes the data into the object from the json reader.
        /// </summary>
        /// <param name="jsonReader">The json reader</param>
        /// <param name="target">The target object to deserialize the data into</param>
        /// <returns>The populated target object</returns>
        public object DeserializeInto(JsonReader jsonReader, object target)
        {
            return jsonSerializer.DeserializeInto(jsonReader, target);
        }

        #endregion

    }

}