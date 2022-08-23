using System;

namespace Mcma.Utility;

/// <summary>
/// Utility for accessing MCMA-specific environment variables
/// </summary>
public static class McmaEnvironmentVariables
{
    /// <summary>
    /// The prefix used for all MCMA environment variables
    /// </summary>
    public const string Prefix = "MCMA_";

    /// <summary>
    /// Gets the value for an "MCMA_" prefixed environment variable
    /// </summary>
    /// <param name="key">The key for the environment variable. Will be prefixed with <see cref="Prefix"/>.</param>
    /// <param name="required">Flag indicating if an exception should be thrown if the environment variable is not found. Defaults to true.</param>
    /// <returns>The value of the environment variable, if found</returns>
    /// <exception cref="McmaException">Thrown if <see cref="required"/> is true and no environment variable with the given key is found.</exception>
    public static string Get(string key, bool required = true)
    {
        var value = Environment.GetEnvironmentVariable(Prefix + key);
            
        if (value == null && required)
            throw new McmaException($"Required environment variable '{Prefix + key}' not set");

        return value;
    }
}