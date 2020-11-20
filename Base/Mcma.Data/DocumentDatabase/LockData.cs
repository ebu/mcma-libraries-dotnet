﻿using System;

namespace Mcma.Data
{
    public class LockData
    {
        public string MutexHolder { get; set; }
        
        public DateTimeOffset Timestamp { get; set; }
        
        public string VersionId { get; set; }
    }
}