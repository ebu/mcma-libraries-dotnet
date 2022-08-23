using System;
using System.Collections;
using System.Collections.Generic;

namespace Mcma.Serialization;

internal class McmaTypeRegistrations : IMcmaTypeRegistrations, IEnumerable<Type>
{
    private List<Type> Types { get; } = new();

    public IMcmaTypeRegistrations Add<T>() => Add(typeof(T));

    public IMcmaTypeRegistrations Add(Type type)
    {
        if (!Types.Contains(type))
            Types.Add(type);
        return this;
    }

    public IEnumerator<Type> GetEnumerator() => Types.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}