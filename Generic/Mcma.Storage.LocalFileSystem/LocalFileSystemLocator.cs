using System;
using Mcma.Model;

namespace Mcma.Storage.LocalFileSystem;

public class LocalFileSystemLocator : Locator
{
    public LocalFileSystemLocator()
    {
        Uri = new Lazy<Uri>(() => new Uri(Url));
    }
        
    private Lazy<Uri> Uri { get; }

    public string LocalPath => Uri.Value.LocalPath;
}