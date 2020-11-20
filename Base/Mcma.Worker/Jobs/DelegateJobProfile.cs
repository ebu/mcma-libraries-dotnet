﻿using System;
using System.Threading.Tasks;

namespace Mcma.Worker
{
    internal class DelegateJobProfile<TJob> : IJobProfile<TJob> where TJob : Job
    {
        public DelegateJobProfile(string name, Func<ProcessJobAssignmentHelper<TJob>, McmaWorkerRequestContext, Task> handler)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public string Name { get; }

        private Func<ProcessJobAssignmentHelper<TJob>, McmaWorkerRequestContext, Task> Handler { get; }

        public Task ExecuteAsync(ProcessJobAssignmentHelper<TJob> workerJobHelper,
                                 McmaWorkerRequestContext requestContext)
            => Handler(workerJobHelper, requestContext);
    }
}
