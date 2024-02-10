using Mcma.Client.Http;
using Mcma.Logging;
using Mcma.Model;
using Mcma.Model.Jobs;
using Mcma.Serialization;
using Newtonsoft.Json.Linq;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Mcma.Api.Http;

public class McmaApiRequest
{
    private readonly Lazy<McmaTracker?> _tracker;

    private JToken? _jsonBody;

    private Exception? _jsonBodyException;

    public McmaApiRequest(string id,
                          string path,
                          HttpMethod httpMethod,
                          IDictionary<string, string>? headers = null,
                          IDictionary<string, string>? queryStringParameters = null,
                          byte[]? body = null)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));

        Id = id;
        Path = path ?? throw new ArgumentNullException(nameof(path));
        HttpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));

        Headers = headers ?? new Dictionary<string, string>();
        QueryStringParameters = queryStringParameters ?? new Dictionary<string, string>();
        Body = body;

        _tracker = new(GetTracker, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    
    public string Id { get; }
    
    public string Path { get; }
    
    public HttpMethod HttpMethod { get; }
    
    public IDictionary<string, string> Headers { get; }
    
    public IDictionary<string, string> QueryStringParameters { get; }
    
    public byte[]? Body { get; }

    public IDictionary<string, object?> PathVariables { get; set; } = new Dictionary<string, object?>();

    public bool HasJsonBody
    {
        get
        {
            TryParseBodyAsJson();

            return _jsonBodyException is null;
        }
    }

    public JToken? JsonBody
    {
        get
        {
            if (!HasJsonBody)
                ExceptionDispatchInfo.Throw(_jsonBodyException!);

            return _jsonBody;
        }
    }

    public Exception? JsonBodyException => _jsonBodyException;

    public McmaTracker? Tracker => _tracker.Value;

    public McmaTracker? GetTracker()
    {
        // try to get the tracker from the headers or query string first
        var hasTracker =
            Headers.TryGetValue(McmaHttpHeaders.Tracker, out var tracker) ||
            QueryStringParameters.TryGetValue(McmaHttpHeaders.Tracker, out tracker);

        if (hasTracker && tracker != null)
        {
            try
            {
                var trackerDataJson = Encoding.UTF8.GetString(Convert.FromBase64String(tracker));
                if (!string.IsNullOrWhiteSpace(trackerDataJson))
                    return McmaJson.Parse(trackerDataJson)?.ToMcmaObject<McmaTracker>();
            }
            catch (Exception e)
            {
                Logger.System.Warn($"Failed to convert text in header or query param 'mcmaTracker' to an McmaTracker object. Error: {e}");
            }
        }

        try
        {
            TryParseBodyAsJson();

            // if we didn't find it in the header or query string, try the body
            if (_jsonBody is not JObject jsonBody)
                return null;

            var trackerProp =
                jsonBody.Properties().FirstOrDefault(j => j.Name.Equals(nameof(Job.Tracker), StringComparison.OrdinalIgnoreCase));

            return trackerProp?.Value.ToMcmaObject<McmaTracker>();
        }
        catch (Exception e)
        {
            Logger.System.Warn($"Failed to parse McmaTracker object found in body's 'tracker' property. Error: {e}");
        }

        return null;
    }

    private void TryParseBodyAsJson()
    {
        if (_jsonBody is not null || _jsonBodyException is not null)
            return;

        if (Body == null || Body.Length == 0)
        {
            _jsonBodyException = new Exception("Body is null or empty");
            return;
        }

        try
        {
            _jsonBody = McmaJson.Parse(Encoding.UTF8.GetString(Body));

            if (_jsonBody is null)
                _jsonBodyException = new Exception("Body has json value 'null'");
        }
        catch (Exception e)
        {
            _jsonBodyException = e;
        }
    }
}