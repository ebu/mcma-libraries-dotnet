using System;
using Mcma.Client;
using Mcma.Logging;
using Mcma.Data;

namespace Mcma.Worker
{
    public class ProviderCollection
    {
        private readonly ILoggerProvider _loggerProvider;
        private readonly IResourceManagerProvider _resourceManagerProvider;
        private readonly IDocumentDatabaseTableProvider _dbTableProvider;

        public ProviderCollection(ILoggerProvider loggerProvider,
                                  IResourceManagerProvider resourceManagerProvider,
                                  IDocumentDatabaseTableProvider dbTableProvider)
        {
            _loggerProvider = loggerProvider;
            _resourceManagerProvider = resourceManagerProvider;
            _dbTableProvider = dbTableProvider;
        }

        public ILoggerProvider LoggerProvider
            => _loggerProvider ?? throw new Exception($"{nameof(LoggerProvider)} not available.");

        public IResourceManagerProvider ResourceManagerProvider
            => _resourceManagerProvider ?? throw new Exception($"{nameof(ResourceManagerProvider)} not available.");

        public IDocumentDatabaseTableProvider DbTableProvider
            => _dbTableProvider ?? throw new Exception($"{nameof(DbTableProvider)} not available.");
    }
}