using System.Net;
using Mcma.Logging;
using Mcma.Model;
using Mcma.Serialization;

namespace Mcma.Api.Http;

public class McmaApiRequestContext
{
    private static readonly HttpMethod[] MethodsSupportingRequestBody = [HttpMethod.Post, HttpMethod.Put, HttpMethod.Patch];

    private readonly Lazy<ILogger?> _logger;

    public McmaApiRequestContext(ILoggerProvider loggerProvider, McmaApiRequest request)
    {
        LoggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
        Request = request ?? throw new ArgumentNullException(nameof(request));

        _logger = new(() => loggerProvider.Get(RequestId, Tracker));
    }

    private ILoggerProvider LoggerProvider { get; }

    public McmaApiRequest Request { get; }

    public McmaApiResponse Response { get; } = new();

    public string RequestId => Request.Id;

    public McmaTracker? Tracker => Request.Tracker;

    public ILogger? Logger => _logger.Value;

    public bool HasRequestBody => (Request?.Body?.Length ?? 0) > 0;

    public bool HasJsonRequestBody => HasRequestBody && Request.HasJsonBody;

    public bool MethodSupportsRequestBody
        => MethodsSupportingRequestBody.Any(x => x.Method.Equals(Request.HttpMethod.Method, StringComparison.OrdinalIgnoreCase));

    public string? GetRequestHeaderValue(string header)
        => Request.Headers?.FirstOrDefault(x => x.Key.Equals(header, StringComparison.OrdinalIgnoreCase)).Value;

    public T? GetRequestBody<T>() where T : McmaObject => Request.JsonBody?.ToMcmaObject<T>();

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
}