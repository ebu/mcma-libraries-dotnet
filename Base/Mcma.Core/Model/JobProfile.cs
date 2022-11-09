using Newtonsoft.Json.Linq;

namespace Mcma.Model;

/// <summary>
/// Represents a type of job processing a service can do
/// </summary>
public class JobProfile : McmaResource
{
    /// <summary>
    /// Gets or sets the name of the profile
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of input parameters required by the caller when running a job with this profile
    /// </summary>
    public ICollection<JobParameter> InputParameters { get; set; } = new List<JobParameter>();

    /// <summary>
    /// Gets or sets the collection of output parameters that will be returned when this profile is run against a job
    /// </summary>
    public ICollection<JobParameter> OutputParameters { get; set; } = new List<JobParameter>();
        
    /// <summary>
    /// Gets or sets a collection of optional input parameters for the profile. The caller may or may not provide these, depending on their needs.
    /// </summary>
    public ICollection<JobParameter> OptionalInputParameters { get; set; } = new List<JobParameter>();
        
    /// <summary>
    /// Gets or sets a collection of custom properties for the profile
    /// </summary>
    public JObject? CustomProperties { get; set; }
}