using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mcma.Model;
using Mcma.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Mcma.Serialization;

/// <summary>
/// Helper class for dealing with MCMA json
/// </summary>
public static class McmaJson
{
    /// <summary>
    /// The @type property used in all MCMA objects
    /// </summary>
    public const string TypePropertyName = "@type";
        
    /// <summary>
    /// The default settings, exposed so they can be used with other libraries that also use JSON.NET
    /// </summary>
    /// <returns></returns>
    public static JsonSerializerSettings DefaultSettings() => DefaultSettings(false);

    private static JsonSerializerSettings DefaultSettings(bool preserveCasing)
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateParseHandling = DateParseHandling.DateTimeOffset,
            Converters =
            {
                new StringEnumConverter(),
                new McmaObjectConverter(),
                new McmaExpandoObjectConverter()
            }
        };

        if (!preserveCasing)
            settings.ContractResolver = new McmaCamelCasePropertyNamesContractResolver();

        return settings;
    }

    /// <summary>
    /// Gets a statically-available <see cref="JsonSerializer"/> configured with MCMA defaults
    /// </summary>
    public static JsonSerializer Serializer { get; } = JsonSerializer.CreateDefault(DefaultSettings(false));

    private static JsonSerializer PreserveCasingSerializer { get; } = JsonSerializer.CreateDefault(DefaultSettings(true));

    public static JToken Parse(string json) => JsonConvert.DeserializeObject<JToken>(json, DefaultSettings());

    /// <summary>
    /// Gets the type of serialized object from a <see cref="JObject"/> by looking for a "@type" property and resolving it
    /// </summary>
    /// <param name="jObj">The json object to inspect</param>
    /// <param name="objectType">The expected object type to fall back to, if any</param>
    /// <returns>The type of the serialized object</returns>
    public static Type GetSerializedType(JObject jObj, Type objectType = null)
    {
        var typeProperty = jObj?.Property(TypePropertyName);
            
        if (typeProperty == null)
            return objectType ?? typeof(McmaExpandoObject);
            
        var typeString = typeProperty.Value.Value<string>();

        objectType = McmaTypes.FindType(typeString) ?? typeof(McmaObject);
            
        typeProperty.Remove();

        return objectType;
    }
        
    /// <summary>
    /// Converts json represented as a <see cref="JToken"/> into an object of type <see cref="T"/> using MCMA deserialization
    /// </summary>
    /// <param name="json">The json to convert</param>
    /// <typeparam name="T">The type of object to convert to</typeparam>
    /// <returns>The resulting <see cref="T"/> object</returns>
    public static T ToMcmaObject<T>(this JToken json) => json.ToObject<T>(Serializer);

    /// <summary>
    /// Converts json represented as a <see cref="JToken"/> into an object using MCMA deserialization
    /// </summary>
    /// <param name="json">The json to convert</param>
    /// <param name="type">The type of object to convert to. If not provided, it will be derived used the "@type" property if available.</param>
    /// <returns>The resulting object</returns>
    public static object ToMcmaObject(this JToken json, Type type) => json.ToObject(type, Serializer);

    /// <summary>
    /// Converts an object to a <see cref="JToken"/> using MCMA serialization
    /// </summary>
    /// <param name="obj">The object to convert</param>
    /// <param name="preserveCasing">A flag indicating if the casing of properties on the object should be preserved. Default is false.</param>
    /// <returns></returns>
    public static JToken ToMcmaJson(this object obj, bool preserveCasing = false)
        => JToken.FromObject(obj, preserveCasing ? PreserveCasingSerializer : Serializer);

    /// <summary>
    /// Converts an object to a <see cref="JObject"/> using MCMA serialization
    /// </summary>
    /// <param name="obj">The object to convert</param>
    /// <param name="preserveCasing">A flag indicating if the casing of properties on the object should be preserved. Default is false.</param>
    /// <returns></returns>
    public static JObject ToMcmaJsonObject(this object obj, bool preserveCasing = false)
        => JObject.FromObject(obj, preserveCasing ? PreserveCasingSerializer : Serializer);

    /// <summary>
    /// Converts a <see cref="JObject"/> to a collection of <see cref="KeyValuePair{TKey,TValue}"/>s of strings to objects
    /// </summary>
    /// <param name="jObj"></param>
    /// <returns></returns>
    public static IEnumerable<KeyValuePair<string, object>> ToKeyValuePairs(this JObject jObj)
        => jObj.Select<KeyValuePair<string, JToken>, KeyValuePair<string, object>>(x => new KeyValuePair<string, object>(x.Key, ToKeyValuePairValue(x.Value)));

    /// <summary>
    /// Helper method for reading JSON from a stream
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <returns>The content of the stream parsed as json in a <see cref="JToken"/></returns>
    public static async Task<JToken> ReadJsonFromStreamAsync(this Stream stream)
    {
        using var textReader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(textReader);
        return await JToken.LoadAsync(jsonReader);
    }
        
    internal static bool IsMcmaObject(JObject jObj)
        => jObj.Properties().Any(p => p.Name.Equals(TypePropertyName, StringComparison.OrdinalIgnoreCase));

    internal static object CreateMcmaObject(JObject jObj, JsonSerializer serializer)
        => jObj.ToObject(GetSerializedType(jObj), serializer);

    internal static object ConvertJsonToClr(JToken token, JsonSerializer serializer)
    {
        switch (token.Type)
        {
            case JTokenType.Boolean:
                return token.Value<bool>();
            case JTokenType.Bytes:
                return token.Value<byte[]>();
            case JTokenType.Date:
                return token.Value<DateTimeOffset>();
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

    internal static IDictionary<string, object> GetPropertyDictionary(object value)
        => value.GetType().GetProperties()
                .Where(p => p.Name != nameof(McmaObject.Type) && p.CanRead && p.GetIndexParameters().Length == 0)
                .ToDictionary(GetJsonPropertyName, p => p.GetValue(value));

    internal static string GetJsonPropertyName(PropertyInfo propertyInfo)
    {
        var jsonPropertyAttribute = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
        return jsonPropertyAttribute != null ? jsonPropertyAttribute.PropertyName : propertyInfo.Name;
    }

    internal static void WriteProperties(JsonWriter writer, JsonSerializer serializer, IDictionary<string, object> properties, bool preserveCasing)
    {
        foreach (var keyValuePair in properties)
        {
            if (keyValuePair.Value == null && serializer.NullValueHandling == NullValueHandling.Ignore)
                continue;

            writer.WritePropertyName(preserveCasing ? keyValuePair.Key : keyValuePair.Key.PascalCaseToCamelCase());
            serializer.Serialize(writer, keyValuePair.Value);
        }
    }
        
    private static object ToKeyValuePairValue(JToken token)
        => token switch
        {
            JObject jObj => jObj.ToKeyValuePairs(),
            JArray jArr => jArr.Select(ToKeyValuePairValue),
            JValue jVal => jVal.Value,
            _ => token.ToString()
        };
}