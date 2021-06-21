using Mcma.Serialization;

namespace Mcma.Storage.LocalFileSystem
{
    public static class LocalFileSystemHelper
    {
        public static IMcmaTypeRegistrations AddTypes() => McmaTypes.Add<LocalFileSystemLocator>();
    }
}