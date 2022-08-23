using System;
using System.Collections.Generic;
using System.Linq;
using Mcma.Logging;
using Mcma.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mcma.Serialization;

/// <summary>
/// Converts objects that inherit from <see cref="McmaObject"/> to and from json
/// </summary>
public class McmaObjectConverter : JsonConverter
{
    /// <summary>
    /// Checks that object inherits from <see cref="McmaObject"/>
    /// </summary>
    /// <param name="objectType">The type to check</param>
    /// <returns>True if the object inherits from <see cref="McmaObject"/>; otherwise, false</returns>
    public override bool CanConvert(Type objectType) => typeof(McmaObject).IsAssignableFrom(objectType);

    /// <summary>
    /// Reads an object derived from <see cref="McmaObject"/> from json
    /// </summary>
    /// <param name="reader">The reader containing the json to deserialize</param>
    /// <param name="objectType">The expected object type</param>
    /// <param name="existingValue">The existing value (not used)</param>
    /// <param name="serializer">The json serializer</param>
    /// <returns>A <see cref="McmaExpandoObject"/></returns>
    /// <exception cref="McmaException">An wrapping exception thrown when deserialization fails, containing the original exception and the type that had a problem deserializing</exception>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        Type serializedType = null;
        try
        {
            var jObj = JObject.Load(reader);

            serializedType = McmaJson.GetSerializedType(jObj, objectType);
            var dynamicObj = (IDictionary<string, object>)Activator.CreateInstance(serializedType);
                
            if (dynamicObj is McmaObject mcmaObj && serializedType == typeof(McmaObject))
                mcmaObj.Type = jObj[McmaJson.TypePropertyName]?.Value<string>();

            foreach (var jsonProp in jObj.Properties().Where(p => !p.Name.Equals(McmaJson.TypePropertyName, StringComparison.OrdinalIgnoreCase)))
                if (!TryReadClrProperty(serializedType, dynamicObj, serializer, jsonProp))
                    dynamicObj[jsonProp.Name] = McmaJson.ConvertJsonToClr(jsonProp.Value, serializer);

            return dynamicObj;
        }
        catch (Exception ex)
        {
            throw new McmaException($"An error occurred reading JSON for an object of type {(serializedType ?? objectType).Name}. See inner exception for details.", ex);
        }
    }

    /// <summary>
    /// Writes an object derived from <see cref="McmaObject"/> as json
    /// </summary>
    /// <param name="writer">The writer used to write the json</param>
    /// <param name="value">The <see cref="McmaExpandoObject"/> to be written</param>
    /// <param name="serializer">The json serializer</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(McmaJson.TypePropertyName);
        writer.WriteValue(((McmaObject)value).Type);

        McmaJson.WriteProperties(writer, serializer, McmaJson.GetPropertyDictionary(value), false);

        McmaJson.WriteProperties(writer, serializer, (IDictionary<string, object>)value, false);

        writer.WriteEndObject();
    }

    private static bool TryReadClrProperty(Type objectType, object obj, JsonSerializer serializer, JProperty jsonProp)
    {
        var clrProp =
            objectType.GetProperties()
                      .FirstOrDefault(p => p.CanWrite && McmaJson.GetJsonPropertyName(p).Equals(jsonProp.Name, StringComparison.OrdinalIgnoreCase));
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
}