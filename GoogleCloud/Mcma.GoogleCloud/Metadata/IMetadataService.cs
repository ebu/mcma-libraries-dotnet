﻿using System.Threading.Tasks;

namespace Mcma.GoogleCloud.Metadata
{
    public interface IMetadataService
    {
        Task<string[]> GetAsync(string path);
    }
}