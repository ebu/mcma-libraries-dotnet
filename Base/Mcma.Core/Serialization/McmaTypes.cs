using System.Collections.Concurrent;
using System.Reflection;
using Mcma.Logging;
using Mcma.Model;

namespace Mcma.Serialization;

/// <summary>
/// Static registry of types of which MCMA serialization must be aware
/// </summary>
public static class McmaTypes
{
    static McmaTypes()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            AddTypesFromAssembly(assembly);

        AppDomain.CurrentDomain.AssemblyLoad += (_, args) => AddTypesFromAssembly(args.LoadedAssembly);
    }

    private static McmaTypeRegistrations Types { get; } = [];

    private static ConcurrentDictionary<(string? TypeString, Type? ObjectType, Type? RootObjectType), Type?> ResolvedTypes { get; } = new();

    private static void AddTypesFromAssembly(Assembly assembly)
    {
        if (assembly.FullName == null || assembly.FullName.StartsWith("System") || assembly.FullName.StartsWith("Microsoft") || assembly.IsDynamic)
            return;

        try
        {
            foreach (var mcmaType in assembly.GetExportedTypes().Where(t => typeof(McmaObject).IsAssignableFrom(t)))
                Add(mcmaType);
        }
        catch (Exception ex)
        {
            Logger.System.Warn($"Failed to load types from assembly {assembly.FullName}.{Environment.NewLine}Exception:{Environment.NewLine}{ex}");
        }
    }

    private static Type? PickBestTypeBasedOnRoot(Type? rootObjectType, Type[] types)
    {
        if (rootObjectType?.FullName is null)
            return null;

        var rootTypeNameParts = rootObjectType.FullName.Split('.');

        var highScore = 0;
        var highScoreTypes = new List<Type>();

        foreach (var type in types)
        {
            if (type.FullName is null)
                continue;

            var typeNameParts = type.FullName.Split('.');

            var score = 0;
            while (score < rootTypeNameParts.Length && score < typeNameParts.Length && typeNameParts[score] == rootTypeNameParts[score])
                score++;

            if (score > highScore)
            {
                highScore = score;
                highScoreTypes = [type];
            }
            else if (score == highScore)
                highScoreTypes.Add(type);
        }

        return highScore > 0 && highScoreTypes.Count == 1 ? highScoreTypes[0] : null;
    }

    /// <summary>
    /// Adds a well-known type to the registry
    /// </summary>
    /// <typeparam name="T">The type to add</typeparam>
    /// <returns></returns>
    public static IMcmaTypeRegistrations Add<T>() => Add(typeof(T));

    /// <summary>
    /// Adds a well-known type to the registry
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IMcmaTypeRegistrations Add(Type type) => Types.Add(type);

    /// <summary>
    /// Finds a registered type with the given name
    /// </summary>
    /// <param name="typeString">The name of the type to find. Must be an unqualified name (<see cref="Type.Name"/>), as would be found in the @type json property</param>
    /// <returns>The type with the given name, if any</returns>
    public static Type? FindType(string? typeString, Type? objectType = null, Type? rootObjectType = null)
        => ResolvedTypes.GetOrAdd((typeString, objectType, rootObjectType), x =>
        {
            var (typeString, objectType, rootObjectType) = x;

            if (typeString == null)
                return null;

            // if the provided type name matches the type from the json, just use that
            if (objectType != null && string.Equals(typeString, objectType.Name, StringComparison.OrdinalIgnoreCase))
                return objectType;

            // check for match in explicitly-provided type collection, then check for match in core types
            var matchingRegisteredTypes = Types.Where(t => t.Name.Equals(typeString, StringComparison.OrdinalIgnoreCase)).ToArray();

            return matchingRegisteredTypes.Length switch
            {
                0 => Type.GetType(typeof(McmaObject).AssemblyQualifiedName?.Replace(nameof(McmaObject), typeString) ?? typeString),
                1 => matchingRegisteredTypes[0],
                _ => PickBestTypeBasedOnRoot(rootObjectType, matchingRegisteredTypes) ??
                     throw new McmaException($"The type name '{typeString}' is ambiguous between the following types:" + Environment.NewLine +
                                             string.Join(Environment.NewLine, matchingRegisteredTypes.Select(t => t.AssemblyQualifiedName)))

            };
        });
}