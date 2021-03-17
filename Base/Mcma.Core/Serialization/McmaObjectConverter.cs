﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mcma.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mcma.Serialization
{
    public class McmaObjectConverter : McmaJsonConverter
    {
        public override bool CanConvert(Type objectType) => typeof(McmaObject).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                var jObj = JObject.Load(reader);

                var serializedType = GetSerializedType(jObj, objectType);
                var dynamicObj = (IDictionary<string, object>)Activator.CreateInstance(serializedType);
                
                if (dynamicObj is McmaObject mcmaObj && serializedType == typeof(McmaObject))
                    mcmaObj.Type = jObj[McmaJson.TypePropertyName]?.Value<string>();

                foreach (var jsonProp in jObj.Properties().Where(p => !p.Name.Equals(McmaJson.TypePropertyName, StringComparison.OrdinalIgnoreCase)))
                    if (!TryReadClrProperty(serializedType, dynamicObj, serializer, jsonProp))
                        dynamicObj[jsonProp.Name] = ConvertJsonToClr(jsonProp.Value, serializer);

                return dynamicObj;
            }
            catch (Exception ex)
            {
                throw new McmaException($"An error occurred reading JSON for an object of type {objectType.Name}. See inner exception for details.", ex);
            }
        }

        private static bool TryReadClrProperty(Type objectType, object obj, JsonSerializer serializer, JProperty jsonProp)
        {
            var clrProp =
                objectType.GetProperties()
                          .FirstOrDefault(p => p.CanWrite && GetJsonPropertyName(p).Equals(jsonProp.Name, StringComparison.OrdinalIgnoreCase));
            if (clrProp == null)
                return false;
            
            try
            {
                clrProp.SetValue(obj, jsonProp.Value.Type != JTokenType.Null ? jsonProp.Value.ToObject(clrProp.PropertyType, serializer) : null);
                return true;
            }
            catch (Exception ex)
            {
                Logger.System.Error($"Failed to set property {clrProp.Name} on type {objectType.Name} with JSON value {jsonProp.Value}: {ex}");
            }

            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(McmaJson.TypePropertyName);
            writer.WriteValue(((McmaObject)value).Type);

            WriteProperties(writer, serializer, GetPropertyDictionary(value), false);

            WriteProperties(writer, serializer, (IDictionary<string, object>)value, false);

            writer.WriteEndObject();
        }
    }
}