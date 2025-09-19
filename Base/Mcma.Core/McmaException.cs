using System.Runtime.Serialization;
using Mcma.Serialization;

namespace Mcma;

/// <summary>
/// Exception thrown by MCMA libraries
/// </summary>
[Serializable]
public sealed class McmaException : Exception
{
    /// <summary>
    /// Instantiates an <see cref="McmaException"/> with an inner exception and a cause
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="cause">The root cause of the error (i.e. inner exception)</param>
    /// <param name="context">An arbitrary object that will be serialized to json and added to the <see cref="Exception.Data"/> property, if provided</param>
    public McmaException(string message, Exception? cause = null, object? context = null)
        : base(message, cause)
    {
        if (context != null)
            Data.Add("Context", context.ToMcmaJson().ToString());
    }
        
    private McmaException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}