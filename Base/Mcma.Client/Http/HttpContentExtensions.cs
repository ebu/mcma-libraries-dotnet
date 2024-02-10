#if NET48_OR_GREATER
using System.Net.Http;
#endif
using Mcma.Serialization;
using Newtonsoft.Json.Linq;

namespace Mcma.Client.Http;

public static class HttpContentExtensions
{
    public static async Task<JToken> ReadAsJsonAsync(this HttpContent content)
    {
        var responseBody = await content.ReadAsStringAsync();

        return !string.IsNullOrWhiteSpace(responseBody) && McmaJson.Parse(responseBody) is JToken jToken
            ? jToken
            : JValue.CreateNull();
    }
}