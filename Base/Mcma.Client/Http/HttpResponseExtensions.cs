using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Serialization;
using Newtonsoft.Json;

namespace Mcma.Client.Http;

public static class HttpResponseExtensions
{
    public static async Task<HttpResponseMessage> ThrowIfFailedAsync(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return response;

        // start with a basic error message that just indicates the response code (similar to EnsureSuccessStatusCode)
        var errorMessage =
            $"Request to {response.RequestMessage.RequestUri} returned an HTTP status code of {response.StatusCode} ({response.ReasonPhrase})";

        if (response.Content == null)
            throw new McmaException(errorMessage);
            
        try
        {
            // try to read the body of the response, in the case that is has more information about what failed
            var errorBody = await response.Content.ReadAsStringAsync();
                    
            if (!string.IsNullOrWhiteSpace(errorBody))
            {
                // in the case that the response body is json, try to parse it to a JToken so we can better format it in the exception
                try
                {
                    errorBody = McmaJson.Parse(errorBody).ToString(Formatting.Indented);
                }
                catch
                {
                    // failed parse as JSON - nothing to be done
                }
                    
                // if we have a body, append the body (as text) to the 
                errorMessage += Environment.NewLine + "Response body:" + Environment.NewLine + errorBody;
            }
        }
        catch
        {
            // something unexpected happened, so we'll just use the message as-is
        }

        // throw the exception with the error message we've built
        throw new McmaException(errorMessage);
    }
}