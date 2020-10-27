using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Mcma.Client
{
    public static class HttpContentExtensions
    {
        public static async Task<JToken> ReadAsJsonAsync(this HttpContent content)
        {
            var responseBody = await content.ReadAsStringAsync();

            return !string.IsNullOrWhiteSpace(responseBody) ? JToken.Parse(responseBody) : JValue.CreateNull();
        }
    }
}