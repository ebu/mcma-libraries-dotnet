namespace Mcma.Utility;

/// <summary>
/// Utility extensions for reading streams
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Reads all bytes in a given stream by copying to a <see cref="MemoryStream"/> and reading out its contents into a byte array
    /// </summary>
    /// <param name="stream">The stream to read</param>
    /// <returns>The array of bytes read from the source stream</returns>
    public static async Task<byte[]> ReadAllBytesAsync(this Stream stream)
    {
        if (stream.CanSeek)
            stream.Position = 0;
            
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
            
        if (stream.CanSeek)
            stream.Position = 0;
            
        return memoryStream.ToArray();
    }
}