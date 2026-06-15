using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Mcma.Serialization;

internal class McmaJsonSerializer : JsonSerializer
{
    private readonly ConcurrentDictionary<Type, McmaJsonSerializer> _typed = [];
    private readonly JsonSerializerSettings _settings;
    private readonly Type? _rootType;
    
    public McmaJsonSerializer(bool preserveCasing = false)
        : this(McmaJson.DefaultSettings(preserveCasing), null)
    {
    }

    private McmaJsonSerializer(JsonSerializerSettings settings, Type? rootType)
    {
        _settings = settings;
        _rootType = rootType;

        CheckAdditionalContent = true;

        ApplySettings();
    }

    private void ApplySettings()
    {
        NullValueHandling = _settings.NullValueHandling;
        ReferenceLoopHandling = _settings.ReferenceLoopHandling;
        DateParseHandling = _settings.DateParseHandling;

        foreach (var converter in _settings.Converters)
            Converters.Add(
                _rootType is Type rootType && converter is IMcmaRootTypeAwareConverter rootTypeAware
                    ? rootTypeAware.ForRootType(rootType)
                    : converter);

        if (_settings.ContractResolver != null)
            ContractResolver = _settings.ContractResolver;
    }

    public McmaJsonSerializer For<T>()
        => For(typeof(T));

    public McmaJsonSerializer For(Type type)
        => _typed.GetOrAdd(type, t => new(_settings, t));
}
