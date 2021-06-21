using System.IO;
using System.Threading.Tasks;
using Mcma.Serialization;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;

namespace Mcma.Data.Azure.CosmosDb
{
    public static class ResponseMessageExtensions
    {
        public static async Task<T> UnwrapResourceAsync<T>(this ResponseMessage responseMessage) where T : class
            => (await responseMessage.ToObjectAsync<CosmosDbItem<T>>())?.Resource;
        
        public static async Task<T> ToObjectAsync<T>(this ResponseMessage responseMessage) where T : class
        {
            string bodyText;
            using (var streamReader = new StreamReader(responseMessage.Content))
                bodyText = await streamReader.ReadToEndAsync();

            return McmaJson.Parse(bodyText).ToMcmaObject<T>();
        }
    }
}