using Newtonsoft.Json.Linq;

namespace Mcma.Api.Http;

public class McmaApiResponse
{
    public int StatusCode { get; set; }

    public string? ErrorMessage { get; set; }

    public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    public byte[]? Body { get; set; }

    public JToken? JsonBody { get; set; }
}