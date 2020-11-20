﻿using System;
using System.IO;
using System.Threading.Tasks;
using Google.Api;
using Mcma.GoogleCloud.Metadata;

namespace Mcma.GoogleCloud.Resources
{
    internal class MonitoredResourceProvider : IMonitoredResourceProvider
    {
        private const string KubernetesNamespaceIdPath = "/var/run/secrets/kubernetes.io/serviceaccount/namespace";
        
        public MonitoredResourceProvider(IMetadataService metadataService)
        {
            MetadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
            CurrentResourceTask = new Lazy<Task<MonitoredResource>>(InternalGetCurrentResourceAsync);
        }
        
        private IMetadataService MetadataService { get; }
        
        private Lazy<Task<MonitoredResource>> CurrentResourceTask { get; }
        
        private MonitoredResource GlobalResource { get; } = new MonitoredResource { Type = "global" };

        private MonitoredResource GetCloudFunctionResource()
            => new MonitoredResource
            {
                Type = "cloud_function",
                Labels =
                {
                    ["function_name"] = CloudEnvironmentVariableHelper.CloudFunctionName,
                    ["region"] = CloudEnvironmentVariableHelper.CloudFunctionRegion
                }
            };

        private async Task<MonitoredResource> GetAppEngineResourceAsync()
            => new MonitoredResource
            {
                Type = "gae_app",
                Labels =
                {
                    ["module_id"] = CloudEnvironmentVariableHelper.AppEngineModuleId,
                    ["version_id"] = CloudEnvironmentVariableHelper.AppEngineVersionId,
                    ["zone"] = await MetadataService.GetInstanceZoneAsync()
                }
            };

        private async Task<MonitoredResource> GetKubernetesEngineResourceAsync()
            => new MonitoredResource
            {
                Type = "k8s_container",
                Labels =
                {
                    ["cluster_name"] = await MetadataService.GetKubernetesClusterNameAsync(),
                    ["namespace_name"] = File.ReadAllText(KubernetesNamespaceIdPath)
                }
            };

        private async Task<MonitoredResource> GetComputeEngineResourceAsync()
            => new MonitoredResource
            {
                Type = "gce_instance",
                Labels =
                {
                    ["instance_id"] = await MetadataService.GetInstanceIdAsync(),
                    ["zone"] = await MetadataService.GetInstanceZoneAsync()
                }
            };

        public Task<MonitoredResource> GetCurrentResourceAsync()
            => CurrentResourceTask.Value;
        
        private async Task<MonitoredResource> InternalGetCurrentResourceAsync()
        {
            if (!await MetadataService.IsAvailableAsync())
                return GlobalResource;

            if (CloudEnvironmentVariableHelper.IsAppEngine)
                return await GetAppEngineResourceAsync();
            
            if (CloudEnvironmentVariableHelper.IsCloudFunction)
                return GetCloudFunctionResource();

            if (await MetadataService.IsOnKubernetesClusterAsync())
                return await GetKubernetesEngineResourceAsync();
            
            return await GetComputeEngineResourceAsync();
        }
    }
}