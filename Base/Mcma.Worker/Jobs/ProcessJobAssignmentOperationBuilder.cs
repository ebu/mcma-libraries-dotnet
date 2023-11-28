using System;
using System.Threading.Tasks;
using Mcma.Model.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Worker.Jobs;

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

    public ProcessJobAssignmentOperationBuilder<TJob> AddProfile<TProfile, TOptions>(Action<TOptions> configure)
        where TProfile : class, IJobProfile<TJob>
        where TOptions : class
    {
        Services.Configure(configure);
        AddProfile<TProfile>();
        return this;
    }

    public ProcessJobAssignmentOperationBuilder<TJob> AddProfile(string profileName,
                                                                 Func<ProcessJobAssignmentHelper<TJob>, McmaWorkerRequestContext, Task> profileHandler)
    {
        Services.AddSingleton<IJobProfile<TJob>>(new DelegateJobProfile<TJob>(profileName, profileHandler));
        return this;
    }
}