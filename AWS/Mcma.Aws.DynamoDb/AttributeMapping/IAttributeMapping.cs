﻿using System;

namespace Mcma.Aws.DynamoDb
{
    public interface IAttributeMapping
    {
        string Name { get; }
        
        Type ResourceType { get; }
        
        object Get(string partitionKey, string sortKey, object input);
    }
}