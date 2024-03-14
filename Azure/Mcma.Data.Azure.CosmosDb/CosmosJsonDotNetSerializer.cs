using System;
using System.IO;
using System.Text;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace Mcma.Data.Azure.CosmosDb;

internal sealed class CosmosJsonDotNetSerializer : CosmosSerializer
{
    private static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);

    internal CosmosJsonDotNetSerializer(JsonSerializerSettings jsonSerializerSettings)
    {
        SerializerSettings = jsonSerializerSettings ?? throw new ArgumentNullException(nameof(jsonSerializerSettings));
    }
    
    private JsonSerializerSettings SerializerSettings { get; }

    public override T FromStream<T>(Stream stream)
    {
        if (typeof(Stream).IsAssignableFrom(typeof(T)))
            return (T)(object)stream;
        
        using (stream)
        {
            using var sr = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(sr);
            
            return GetSerializer().Deserialize<T>(jsonTextReader);
        }
    }

    public override Stream ToStream<T>(T input)
    {
        var streamPayload = new MemoryStream();
        
        using var streamWriter = new StreamWriter(streamPayload, encoding: DefaultEncoding, bufferSize: 1024, leaveOpen: true);
        using var writer = new JsonTextWriter(streamWriter);

        writer.Formatting = Formatting.None;
        
        GetSerializer().Serialize(writer, input);
        
        writer.Flush();
        streamWriter.Flush();

        streamPayload.Position = 0;
        
        return streamPayload;
    }
    
    private JsonSerializer GetSerializer() => JsonSerializer.Create(SerializerSettings);
}