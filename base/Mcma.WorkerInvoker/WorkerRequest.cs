using System.Collections.Generic;
using Mcma;

namespace Mcma.WorkerInvoker
{
    public class WorkerRequest
    {
        public string OperationName { get; set; }

        public object Input { get; set; }

        public McmaTracker Tracker { get; set; }
    }
}
