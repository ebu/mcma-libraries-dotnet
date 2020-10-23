﻿using System;
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

        public McmaApiRequestContext(McmaApiRequest request, ILoggerProvider loggerProvider = null, IEnvironmentVariables environmentVariables = null)
        {
            Request = request;
            LoggerProvider = loggerProvider;
            EnvironmentVariables = environmentVariables ?? Mcma.EnvironmentVariables.Instance;
        }

        public McmaApiRequest Request { get; }

        private ILoggerProvider LoggerProvider { get; }

        public IEnvironmentVariables EnvironmentVariables { get; }

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
                Request.JsonBody = JToken.Parse(Encoding.UTF8.GetString(Request.Body));
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public T GetRequestBody<T>() where T : McmaObject => Request?.JsonBody?.ToMcmaObject<T>();

        public void SetResponseStatusCode(HttpStatusCode status, string statusMessage = null)
            => SetResponseStatus((int)status, statusMessage);

        public void SetResponseStatus(int status, string statusMessage = null)
        {
            Response.StatusCode = status;
            Response.StatusMessage = statusMessage;
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
                        return JToken.Parse(trackerDataJson).ToMcmaObject<McmaTracker>();
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
