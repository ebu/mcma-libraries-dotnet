using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Worker
{
    public class ProcessJobAssignmentOperationBuilder<TJob> where TJob : Job
    {
        public ProcessJobAssignmentOperationBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
        
        public IServiceCollection Services { get; }

        public ProcessJobAssignmentOperationBuilder<TJob> AddProfile<TProfile>()
            where TProfile : class, IJobProfile<TJob>
        {
            Services.AddSingleton<IJobProfile<TJob>, TProfile>();
            return this;
        }

        public ProcessJobAssignmentOperationBuilder<TJob> AddProfile(string profileName,
                                                                     Func<ProcessJobAssignmentHelper<TJob>, McmaWorkerRequestContext, Task> profileHandler)
        {
            Services.AddSingleton<IJobProfile<TJob>>(new DelegateJobProfile<TJob>(profileName, profileHandler));
            return this;
        }
    }
}