﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mcma.Client;
using Mcma.Data;
using Mcma.Logging;
using Microsoft.Extensions.Options;

namespace Mcma.Worker
{
    public class ProcessJobAssignmentOperation<TJob> : McmaWorkerOperation<ProcessJobAssignmentRequest> where TJob : Job
    {
        internal ProcessJobAssignmentOperation(IDocumentDatabaseTableProvider dbTableProvider,
                                               IResourceManagerProvider resourceManagerProvider,
                                               IOptions<McmaWorkerOptions> options)
        {
            DbTableProvider = dbTableProvider ?? throw new ArgumentNullException(nameof(dbTableProvider));
            ResourceManagerProvider = resourceManagerProvider ?? throw new ArgumentNullException(nameof(resourceManagerProvider));
            McmaWorkerOptions = options.ValidateAndGet();
        }

        private IDocumentDatabaseTableProvider DbTableProvider { get; }

        private IResourceManagerProvider ResourceManagerProvider { get; }

        private McmaWorkerOptions McmaWorkerOptions { get; }

        internal List<IJobProfile<TJob>> Profiles { get; } = new List<IJobProfile<TJob>>();

        private JobStatus InitialJobStatus { get; set; } = JobStatus.Running;

        public override string Name => "ProcessJobAssignment";

        public ProcessJobAssignmentOperation<TJob> SetInitialJobStatus(JobStatus initialJobStatus)
        {
            InitialJobStatus = initialJobStatus;
            return this;
        }

        protected override async Task ExecuteAsync(McmaWorkerRequestContext requestContext, ProcessJobAssignmentRequest requestInput)
        {
            if (requestContext == null) throw new ArgumentNullException(nameof(requestContext));
            if (requestInput == null) throw new ArgumentNullException(nameof(requestInput));

            var logger = requestContext.Logger;

            var jobAssignmentHelper =
                new ProcessJobAssignmentHelper<TJob>(
                    await DbTableProvider.GetAsync(McmaWorkerOptions.TableName),
                    ResourceManagerProvider.Get(McmaWorkerOptions.ResourceManager),
                    requestContext);

            try
            {
                logger.Debug("Initializing job helper...");

                await jobAssignmentHelper.InitializeAsync(InitialJobStatus);
                
                logger.Info("Validating job...");

                var matchedProfile = Profiles.FirstOrDefault(p => p.Name.Equals(jobAssignmentHelper.Profile.Name, StringComparison.OrdinalIgnoreCase));
                if (matchedProfile == null)
                {
                    await FailJobAsync(logger, jobAssignmentHelper, $"Job profile {jobAssignmentHelper.Profile?.Name} is not supported.");
                    return;
                }
                
                jobAssignmentHelper.ValidateJob(Profiles.Select(p => p.Name));

                logger.Info($"Found handler for job profile '{matchedProfile.Name}'"); 
                
                await matchedProfile.ExecuteAsync(jobAssignmentHelper, requestContext);

                logger.Info($"Handler for job profile '{matchedProfile.Name}' completed");
            }
            catch (Exception ex)
            {
                logger.Error($"An error occurred executing request for operation '{requestContext.OperationName}'", ex);
                await FailJobAsync(logger, jobAssignmentHelper, ex.Message);
            }
        }

        private async Task FailJobAsync(ILogger logger, ProcessJobAssignmentHelper<TJob> jobAssignmentHelper, string message)
        {
            try {
                await jobAssignmentHelper.FailAsync(new ProblemDetail
                {
                    Type = "uri://mcma.ebu.ch/rfc7807/generic-job-failure",
                    Title = "Generic job failure",
                    Detail = message
                });
            }
            catch (Exception innerEx)
            {
                logger.Error("An error occurred marking the job assignment failed.", innerEx);
            }
        }
    }
}
