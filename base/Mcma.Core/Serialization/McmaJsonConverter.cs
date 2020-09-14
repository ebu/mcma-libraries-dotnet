using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mcma.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mcma.Serialization
{
    public abstract class McmaJsonConverter : JsonConverter
    {
        protected Type GetSerializedType(JObject jObj, Type objectType) => GetSerializedType(jObj.Property(McmaJson.TypePropertyName), objectType);

        protected static Type GetSerializedType(JProperty typeProperty, Type objectType)
        {
            if (typeProperty == null)
                return objectType ?? typeof(McmaExpandoObject);
            
            var typeString = typeProperty.Value.Value<string>();

            objectType = McmaTypes.FindType(typeString) ?? typeof(McmaObject);
            
            typeProperty.Remove();

            return objectType;
        }

        protected bool IsMcmaObject(JObject jObj)
            => jObj.Properties().Any(p => p.Name.Equals(McmaJson.TypePropertyName, StringComparison.OrdinalIgnoreCase));

        protected object CreateMcmaObject(JObject jObj, JsonSerializer serializer)
            => jObj.ToObject(GetSerializedType(jObj, null), serializer);

        protected object ConvertJsonToClr(JToken token, JsonSerializer serializer)
        {
            switch (token.Type)
            {
                case JTokenType.Boolean:
                    return token.Value<bool>();
                case JTokenType.Bytes:
                    return token.Value<byte[]>();
                case JTokenType.Date:
                    return token.Value<DateTime>();
                case JTokenType.Float:
                    return token.Value<decimal>();
                case JTokenType.Guid:
                    return token.Value<Guid>();
                case JTokenType.Integer:
                    return token.Value<long>();
                case JTokenType.String:
                case JTokenType.Uri:
                    return token.Value<string>();
                case JTokenType.TimeSpan:
                    return token.Value<TimeSpan>();
                case JTokenType.Null:
                case JTokenType.Undefined:
                    return null;
                case JTokenType.Array:
                    return token.Select(x => ConvertJsonToClr(x, serializer)).ToArray();
                case JTokenType.Object:
                    var jObj = (JObject)token;
                    return IsMcmaObject(jObj) ? CreateMcmaObject(jObj, serializer) : jObj.ToObject<McmaExpandoObject>(serializer);
                default:
                    return token;
            }
        }

        protected static IDictionary<string, object> GetPropertyDictionary(object value)
            => value.GetType().GetProperties()
                    .Where(p => p.Name != nameof(McmaObject.Type) && p.CanRead && p.GetIndexParameters().Length == 0)
                    .ToDictionary(GetJsonPropertyName, p => p.GetValue(value));

        protected static string GetJsonPropertyName(PropertyInfo propertyInfo)
        {
            var jsonPropertyAttribute = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
            return jsonPropertyAttribute != null ? jsonPropertyAttribute.PropertyName : propertyInfo.Name;
        }

        protected static void WriteProperties(JsonWriter writer, JsonSerializer serializer, IDictionary<string, object> properties, bool preserveCasing)
        {
            foreach (var keyValuePair in properties)
            {
                if (keyValuePair.Value == null && serializer.NullValueHandling == NullValueHandling.Ignore)
                    continue;

                writer.WritePropertyName(preserveCasing ? keyValuePair.Key : keyValuePair.Key.PascalCaseToCamelCase());
                serializer.Serialize(writer, keyValuePair.Value);
            }
        }
    }
}