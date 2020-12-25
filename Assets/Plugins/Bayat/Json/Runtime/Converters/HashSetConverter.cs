using Bayat.Core.Reflection;
using Bayat.Json.Linq;
using Bayat.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Bayat.Json.Converters
{
    public class HashSetConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializerWriter internalWriter)
        {
            //throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializerReader internalReader)
        {
            var shouldReplace = internalReader.Serializer.ObjectCreationHandling == ObjectCreationHandling.Replace;

            //Return the existing value (or null if it doesn't exist)
            if (reader.TokenType == JsonToken.Null)
                return shouldReplace ? null : existingValue;

            //Dynamically create the HashSet
            var result = !shouldReplace && existingValue != null
                ? existingValue
                : Activator.CreateInstance(objectType);

            var genericType = objectType.GetGenericArguments()[0];
            var addMethod = objectType.GetMethod("Add");

            var jo = JArray.Load(reader);

            for (var i = 0; i < jo.Count; i++)
            {
                var itemValue = internalReader.Serializer.Deserialize(jo[i].CreateReader(), genericType);
                addMethod.Invoke(result, new[] { itemValue });
            }

            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType() && objectType.GetGenericTypeDefinition() == typeof(HashSet<>);
        }

        public override bool CanWrite
        {
            get { return false; }
        }


    }
}
