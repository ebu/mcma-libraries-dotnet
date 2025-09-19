using System.Reflection;

namespace Mcma.Utility;

/// <summary>
/// Utility extensions for checking for implicit conversions between types
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Checks if type <see cref="from"/> can be implicitly converted to type <see cref="T"/>
    /// </summary>
    /// <param name="from">The type to convert from</param>
    /// <typeparam name="T">The type to convert to</typeparam>
    /// <returns>True if <see cref="from"/> can be implicitly converted to <see cref="T"/>; otherwise, false</returns>
    public static bool HasImplicitConversionTo<T>(this Type from)
        => @from.HasImplicitConversionTo(typeof(T));

    /// <summary>
    /// Checks if type <see cref="from"/> can be implicitly converted to type <see cref="to"/>
    /// </summary>
    /// <param name="from">The type to convert from</param>
    /// <param name="to">The type to convert to</param>
    /// <returns>True if <see cref="from"/> can be implicitly converted to <see cref="to"/>; otherwise, false</returns>
    public static bool HasImplicitConversionTo(this Type from, Type to)
        =>
            from.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Any(mi =>
                         mi.Name == "op_Implicit" &&
                         mi.ReturnType == to &&
                         mi.GetParameters().Any(pi => pi.ParameterType == from));
}