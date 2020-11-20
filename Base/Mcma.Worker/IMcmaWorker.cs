﻿using System.Threading.Tasks;

namespace Mcma.Worker
{
    public interface IMcmaWorker
    {
        Task DoWorkAsync(McmaWorkerRequestContext requestContext);
    }
}
