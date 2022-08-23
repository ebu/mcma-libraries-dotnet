using System;
using System.IO;
using System.Threading.Tasks;

namespace Mcma.Storage;

public static class StreamHelper
{
    public static long CopyBufferSize { get; set; } = 128 * 1024;

    public static IProgress<long> CreateProgressHandler(long sourceLength, Action<StreamProgress> progressChangedHandler)
        => new Progress<long>(value => progressChangedHandler?.Invoke(new StreamProgress(value, sourceLength)));

    public static IProgress<long> CreateProgressHandler(this Stream source, Action<StreamProgress> progressChangedHandler)
        => CreateProgressHandler(source.CanSeek ? source.Length : long.MaxValue, progressChangedHandler);

    public static async Task CopyToAsync(this Stream source, Stream destination, IProgress<long> progress)
    {
        var buffer = new byte[CopyBufferSize];
            
        while (true)
        {
            var bytesRead = await source.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0)
                break;

            await destination.WriteAsync(buffer, 0, bytesRead);
            progress.Report(bytesRead);
        }
    }
}