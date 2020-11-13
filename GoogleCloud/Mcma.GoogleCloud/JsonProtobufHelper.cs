using System;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json.Linq;

namespace Mcma.GoogleCloud
{
    public static class JsonProtobufHelper
    {
        public static Struct ToProtobufStruct(this JObject jObject)
        {
            var gStruct = new Struct();

            foreach (var jsonProperty in jObject.Properties())
                gStruct.Fields[jsonProperty.Name] = jsonProperty.ToProtobufValue();

            return gStruct;
        }

        private static Value ToProtobufValue(this JToken jToken)
            => jToken switch
            {
                JObject jObject => Value.ForStruct(jObject.ToProtobufStruct()),
                JArray jArray => Value.ForList(jArray.Select(t => ToProtobufValue(t)).ToArray()),
                JValue jValue when jValue.Type == JTokenType.Boolean => Value.ForBool(jValue.Value<bool>()),
                JValue jValue when jValue.Type == JTokenType.Float => Value.ForNumber(jValue.Value<double>()),
                JValue jValue when jValue.Type == JTokenType.Integer => Value.ForNumber(jValue.Value<long>()),
                JValue jValue when jValue.Type == JTokenType.String => Value.ForString(jValue.Value<string>()),
                JValue jValue when jValue.Type == JTokenType.Date => Value.ForString(jValue.Value<DateTimeOffset>().ToString("O")),
                JValue jValue when jValue.Type == JTokenType.TimeSpan => Value.ForString(jValue.Value<TimeSpan>().ToString()),
                JValue jValue when jValue.Type == JTokenType.Null || jValue.Type == JTokenType.Undefined => Value.ForNull(),
                _ => Value.ForString(jToken.ToString())
            };
    }
}