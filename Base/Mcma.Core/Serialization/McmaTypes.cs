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
        if (string.IsNullOrWhiteSpace(rootObjectType?.FullName))
            return null;

        var rootTypeNameParts = rootObjectType!.FullName!.Split('.');

        var highScore = 0;
        var highScoreTypes = new List<Type>();

        foreach (var type in types.Where(t => !string.IsNullOrWhiteSpace(t?.FullName)))
        {
            var typeNameParts = type!.FullName!.Split('.');

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
    /// <param name="objectType">The type of object indicated by code (may be different than what's indicated in by the <see cref="typeString"/>)</param>
    /// <param name="rootObjectType">The type of the root object indicated by code, if known</param>
    /// <returns>The type with the given name, if any</returns>
    public static Type? FindType(string? typeString, Type? objectType = null, Type? rootObjectType = null)
        => ResolvedTypes.GetOrAdd((typeString, objectType, rootObjectType), x =>
        {
            var (typeStringToAdd, objectTypeToAdd, rootObjectTypeToAdd) = x;

            if (typeStringToAdd == null)
                return null;

            // if the provided type name matches the type from the json, just use that
            if (objectTypeToAdd != null && string.Equals(typeStringToAdd, objectTypeToAdd.Name, StringComparison.OrdinalIgnoreCase))
                return objectTypeToAdd;

            // check for match in explicitly-provided type collection, then check for match in core types
            var matchingRegisteredTypes = Types.Where(t => t.Name.Equals(typeStringToAdd, StringComparison.OrdinalIgnoreCase)).ToArray();

            return matchingRegisteredTypes.Length switch
            {
                0 => Type.GetType(typeof(McmaObject).AssemblyQualifiedName?.Replace(nameof(McmaObject), typeStringToAdd) ?? typeStringToAdd),
                1 => matchingRegisteredTypes[0],
                _ => PickBestTypeBasedOnRoot(rootObjectTypeToAdd, matchingRegisteredTypes) ??
                     throw new McmaException($"The type name '{typeStringToAdd}' is ambiguous between the following types:" + Environment.NewLine +
                                             string.Join(Environment.NewLine, matchingRegisteredTypes.Select(t => t.AssemblyQualifiedName)))

            };
        });
}