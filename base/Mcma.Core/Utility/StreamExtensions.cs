using System.IO;
using System.Threading.Tasks;

namespace Mcma.Utility
{
    public static class StreamExtensions
    {
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
}