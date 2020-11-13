using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mcma.Serialization
{
    /// <summary>
    /// Represents a custom MCMA converter for <see cref="McmaExpandoObject"/>s
    /// </summary>
    public class McmaExpandoObjectConverter : JsonConverter
    {
        /// <summary>
        /// Checks that the type is <see cref="McmaExpandoObject"/>
        /// </summary>
        /// <param name="objectType">The type to check</param>
        /// <returns>True if the type is <see cref="McmaExpandoObject"/>; otherwise, false</returns>
        public override bool CanConvert(Type objectType) => objectType == typeof(McmaExpandoObject);

        /// <summary>
        /// Reads an <see cref="McmaExpandoObject"/> from a <see cref="JsonReader"/>
        /// </summary>
        /// <param name="reader">The reader containing the json to deserialize</param>
        /// <param name="objectType">The expected object type</param>
        /// <param name="existingValue">The existing value (not used)</param>
        /// <param name="serializer">The json serializer</param>
        /// <returns>A <see cref="McmaExpandoObject"/></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObj = JObject.Load(reader);

            IDictionary<string, object> expando = new McmaExpandoObject();

            foreach (var jsonProp in jObj.Properties())
                expando[jsonProp.Name] = McmaJson.ConvertJsonToClr(jsonProp.Value, serializer);

            return expando;
        }

        /// <summary>
        /// Writes a <see cref="McmaExpandoObject"/> as json
        /// </summary>
        /// <param name="writer">The writer used to write the json</param>
        /// <param name="value">The <see cref="McmaExpandoObject"/> to be written</param>
        /// <param name="serializer">The json serializer</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            McmaJson.WriteProperties(writer, serializer, (IDictionary<string, object>)value, true);

            writer.WriteEndObject();
        }
    }
}