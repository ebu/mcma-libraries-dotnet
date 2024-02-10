namespace Mcma.Serialization;

/// <summary>
/// Interface for registering types with MCMA serialization
/// </summary>
public interface IMcmaTypeRegistrations
{
    /// <summary>
    /// Adds the type <see cref="T"/> to the types known to MCMA serialization
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IMcmaTypeRegistrations Add<T>();

    /// <summary>
    /// Adds the type <see cref="type"/> to the types known to MCMA serialization
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IMcmaTypeRegistrations Add(Type type);
}