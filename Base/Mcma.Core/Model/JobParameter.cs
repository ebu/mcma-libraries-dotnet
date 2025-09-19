namespace Mcma.Model;

/// <summary>
/// Represents a defined parameter on a job profile
/// </summary>
public class JobParameter
{
    /// <summary>
    /// Gets or sets the name of the parameter
    /// </summary>
    public string? ParameterName { get; set; }

    /// <summary>
    /// Gets or sets the expected type of the parameter's value
    /// </summary>
    public string? ParameterType { get; set; }
}