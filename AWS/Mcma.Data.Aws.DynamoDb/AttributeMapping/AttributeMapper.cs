﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Mcma.Data.Aws.DynamoDb.AttributeMapping;

public class AttributeMapper : IAttributeMapper
{
    public AttributeMapper(IEnumerable<IAttributeMapping> mappings)
    {
        Mappings = mappings?.ToArray() ?? [];
    }

    private IAttributeMapping[] Mappings { get; }

    public Dictionary<string, object> GetMappedAttributes<TResource>(string partitionKey, string sortKey, TResource resource)
    {
        var mappings = Mappings.Where(x => x.ResourceType == typeof(TResource)).ToList();
        if (!mappings.Any())
            return [];

        return mappings
            .ToDictionary(
                x => x.Name, 
                x => x.Get(partitionKey, sortKey, resource));
    }
}