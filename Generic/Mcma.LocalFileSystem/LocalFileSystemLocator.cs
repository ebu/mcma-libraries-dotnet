using System;

namespace Mcma.LocalFileSystem
{
    public class LocalFileSystemLocator : Locator
    {
        public LocalFileSystemLocator()
        {
            Uri = new Lazy<Uri>(() => new Uri(Url));
        }
        
        private Lazy<Uri> Uri { get; }

        public string LocalPath => Uri.Value.LocalPath;
    }
}