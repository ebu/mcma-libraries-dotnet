using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Mcma.Logging;
using Mcma.Serialization;
using Mcma.Client;
using Newtonsoft.Json.Linq;

namespace Mcma.Api
{
    public class McmaApiRequestContext
    {
        private static readonly HttpMethod[] MethodsSupportingRequestBody = {HttpMethod.Post, HttpMethod.Put, new HttpMethod("PATCH")};

        public McmaApiRequestContext(ILoggerProvider loggerProvider, McmaApiRequest request)
        {
            LoggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public McmaApiRequest Request { get; }

        private ILoggerProvider LoggerProvider { get; }

        public McmaApiResponse Response { get; } = new McmaApiResponse();

        public string RequestId => Request?.Id;

        public bool HasRequestBody => (Request?.Body?.Length ?? 0) > 0;

        public bool MethodSupportsRequestBody
            => MethodsSupportingRequestBody.Any(x => x.Method.Equals(Request.HttpMethod.Method, StringComparison.OrdinalIgnoreCase));

        public string GetRequestHeaderValue(string header)
            => Request?.Headers?.FirstOrDefault(x => x.Key.Equals(header, StringComparison.OrdinalIgnoreCase)).Value;

        public bool TryLoadRequestJsonBody(out Exception exception)
        {
            exception = null;
            if (Request.JsonBody != null)
                return true;
            
            try
            {
                Request.JsonBody = McmaJson.Parse(Encoding.UTF8.GetString(Request.Body));
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public T GetRequestBody<T>() where T : McmaObject => Request?.JsonBody?.ToMcmaObject<T>();

        public void SetResponseError(HttpStatusCode status, string errorMessage)
            => SetResponseError((int)status, errorMessage);

        public void SetResponseError(int status, string errorMessage)
        {
            if (status < 400)
                throw new McmaException("McmaApiRequestContext.SetResponseError can only be used to handle 4xx or 5xx errors");
                
            Response.StatusCode = status;
            Response.ErrorMessage = errorMessage;
        }

        public void SetResponseBody(object body) => Response.JsonBody = body?.ToMcmaJson();

        public void SetResponseHeader(string header, string value) => Response.Headers[header] = value;

        public McmaTracker GetTracker()
        {
            string tracker = null;

            // try to get the tracker from the headers or query string first
            var hasTracker =
                (Request?.Headers?.TryGetValue(McmaHttpHeaders.Tracker, out tracker) ?? false) ||
                (Request?.QueryStringParameters?.TryGetValue(McmaHttpHeaders.Tracker, out tracker) ?? false);
            if (hasTracker && tracker != null)
            {
                try
                {
                    var trackerDataJson = Encoding.UTF8.GetString(Convert.FromBase64String(tracker));
                    if (!string.IsNullOrWhiteSpace(trackerDataJson))
                        return McmaJson.Parse(trackerDataJson).ToMcmaObject<McmaTracker>();
                }
                catch (Exception e)
                {
                    LoggerProvider?.Get(RequestId)?.Warn($"Failed to convert text in header or query param 'mcmaTracker' to an McmaTracker object. Error: {e}");
                }
            }

            try
            {
                // if we didn't find it in the header or query string, try the body
                if (!TryLoadRequestJsonBody(out _) || !(Request.JsonBody is JObject jsonBody))
                    return null;

                var trackerProp =
                    jsonBody.Properties().FirstOrDefault(j => j.Name.Equals(nameof(Job.Tracker), StringComparison.OrdinalIgnoreCase));

                return trackerProp?.Value.ToMcmaObject<McmaTracker>();
            }
            catch (Exception e)
            {
                LoggerProvider?.Get(RequestId)?.Warn($"Failed to parse McmaTracker object found in body's 'tracker' property. Error: {e}");
            }

            return null;
        }

        public ILogger GetLogger() => LoggerProvider?.Get(RequestId, GetTracker());
    }
}
