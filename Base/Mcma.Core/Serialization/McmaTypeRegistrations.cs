using System.Collections;
using System.Collections.Concurrent;

namespace Mcma.Serialization;

internal class McmaTypeRegistrations : IMcmaTypeRegistrations, IEnumerable<Type>
{
    private ConcurrentDictionary<Type, Type> Types { get; } = [];

    public IMcmaTypeRegistrations Add<T>() => Add(typeof(T));

    public IMcmaTypeRegistrations Add(Type type)
    {
        Types.TryAdd(type, type);
        return this;
    }

    public IEnumerator<Type> GetEnumerator() => Types.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}