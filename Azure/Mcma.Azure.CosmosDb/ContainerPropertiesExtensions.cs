﻿using System;
using Microsoft.Azure.Cosmos;

namespace Mcma.Azure.CosmosDb
{
    public static class ContainerPropertiesExtensions
    {
        public static string PartitionKeyName(this ContainerProperties containerProperties)
        {
            var partitionKeyParts = containerProperties.PartitionKeyPath.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
            if (partitionKeyParts.Length > 1)
                throw new McmaException(
                    $"Container {containerProperties.Id} defines a partition key with multiple paths ({containerProperties.PartitionKeyPath}). MCMA only supports partition keys with a single path.");

            return partitionKeyParts[0];
        }
    }
}