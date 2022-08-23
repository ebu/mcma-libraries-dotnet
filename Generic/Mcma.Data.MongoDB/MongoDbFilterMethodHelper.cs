using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Mcma.Data.MongoDB;

internal static class MongoDbFilterMethodHelper
{
    private static readonly Type[] FilterableTypes =
    {
        typeof(bool),
        typeof(string),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(decimal),
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(TimeSpan)
    };

    private static Lazy<MethodInfo> OpenGenericFilterMethod { get; } =
        new(() => typeof(MongoDbFilterDefinitionBuilder).GetMethod(nameof(MongoDbFilterDefinitionBuilder.CreateBinaryOperationFilter),
                                                                   BindingFlags.Static | BindingFlags.NonPublic));

    private static readonly Lazy<ConcurrentDictionary<Type, MethodInfo>> GenericFilterMethods =
        new(() => new ConcurrentDictionary<Type, MethodInfo>(
                FilterableTypes.ToDictionary(x => x, x => OpenGenericFilterMethod.Value.MakeGenericMethod(x))));

    public static MethodInfo GetFilterMethod(Type type)
        => GenericFilterMethods.Value.GetOrAdd(type, x => OpenGenericFilterMethod.Value.MakeGenericMethod(x));
}