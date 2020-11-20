﻿using System;

namespace Mcma.Aws.DynamoDb
{
    public readonly struct LocalSecondaryIndexDescription
    {
        public LocalSecondaryIndexDescription(string name, string sortKeyName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            SortKeyName = sortKeyName ?? throw new ArgumentNullException(nameof(sortKeyName));
        }
        
        public string Name { get; }
        
        public string SortKeyName { get; }
    }
}