﻿using Google.Cloud.Functions.Hosting;
using Mcma.GoogleCloud.Firestore;
using Mcma.Worker;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.GoogleCloud.Functions.Worker
{
    public abstract class McmaJobAssignmentWorkerStartup<T> : FunctionsStartup where T : Job
    {
        protected abstract string ApplicationName { get; }

        protected virtual IServiceCollection ConfigureAdditionalServices(IServiceCollection services) => services;

        protected virtual void BuildFirestore(FirestoreTableBuilder builder)
        {
        }

        protected virtual void AddAdditionalOperations(McmaWorkerBuilder builder)
        {
        }

        protected abstract void AddProfiles(ProcessJobAssignmentOperationBuilder<T> builder);

        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
            => ConfigureAdditionalServices(services)
                .AddMcmaGoogleCloudFunctionJobAssignmentWorker<T>(ApplicationName, AddProfiles, BuildFirestore, AddAdditionalOperations);
    }
}