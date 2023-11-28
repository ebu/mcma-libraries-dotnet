using System;
using Mcma.Model;

namespace Mcma.Api.Http;

public class McmaApiError : McmaObject
{
    public McmaApiError(int status = 0, string message = null, string path = null, DateTimeOffset? timestamp = null, string error = null)
    {
        Status = status;
        Message = message;
        Path = path;

        Timestamp = timestamp ?? DateTimeOffset.UtcNow;
        Error = error ?? HttpStatusCodeMessage.From(status);
    }

    public DateTimeOffset Timestamp { get; }

    public int Status { get; }

    public string Error { get; }

    public string Message { get; }

    public string Path { get; }
}