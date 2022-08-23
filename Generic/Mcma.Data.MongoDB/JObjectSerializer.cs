using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Linq;

namespace Mcma.Data.MongoDB;

public class JObjectBsonSerializer : SerializerBase<JObject>
{
    public override JObject Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => JObject.Parse(BsonDocumentSerializer.Instance.Deserialize(context).ToString());

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JObject value)
        => BsonDocumentSerializer.Instance.Serialize(context, global::MongoDB.Bson.BsonDocument.Parse(value.ToString()));
}