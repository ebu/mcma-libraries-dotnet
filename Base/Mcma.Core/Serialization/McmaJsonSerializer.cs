using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Mcma.Serialization;

internal class McmaJsonSerializer : JsonSerializer
{
    private static readonly ConcurrentDictionary<Type, McmaJsonSerializer> Typed = new();

    public McmaJsonSerializer(bool preserveCasing = false)
        : this(McmaJson.DefaultSettings(preserveCasing), null)
    {
    }

    private McmaJsonSerializer(JsonSerializerSettings settings, Type? rootType)
    {
        Settings = settings;
        RootType = rootType;

        ApplySettings();
    }

    private JsonSerializerSettings Settings { get; }

    internal Type? RootType { get; }

    private void ApplySettings()
    {
        CheckAdditionalContent = true;
        
        NullValueHandling = Settings.NullValueHandling;
        ReferenceLoopHandling = Settings.ReferenceLoopHandling;
        DateParseHandling = Settings.DateParseHandling;

        foreach (var converter in Settings.Converters)
            Converters.Add(converter);

        if (Settings.ContractResolver != null)
            ContractResolver = Settings.ContractResolver;
    }

    public McmaJsonSerializer For<T>()
        =>  For(typeof(T));

    public McmaJsonSerializer For(Type type)
        => Typed.GetOrAdd(type, t => new McmaJsonSerializer(Settings, t));
}
