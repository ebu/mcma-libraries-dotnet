using Microsoft.Extensions.Options;

namespace Mcma.Worker
{
    public static class McmaWorkerOptionsExtensions
    {
        public static McmaWorkerOptions ValidateAndGet(this IOptions<McmaWorkerOptions> options)
        {
            if (options.Value == null)
                throw new McmaException("Worker not configured");
            if (string.IsNullOrWhiteSpace(options.Value.TableName))
                throw new McmaException("TableName not configured in worker options");

            return options.Value;
        }
    }
}