using System;

namespace Mcma.Data
{
    public class LockData
    {
        public string MutexHolder { get; set; }
        
        public long Timestamp { get; set; }
        
        public string VersionId { get; set; }
    }
}