using Mcma.Serialization;
using Newtonsoft.Json.Linq;

namespace Mcma.Client.Http;

public static class HttpContentExtensions
{
    public static async Task<JToken> ReadAsJsonAsync(this HttpContent content)
    {
        var responseBody = await content.ReadAsStringAsync();

        return !string.IsNullOrWhiteSpace(responseBody) ? McmaJson.Parse(responseBody) : JValue.CreateNull();
    }
}