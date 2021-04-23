using Mcma.Serialization;

namespace Mcma.LocalFileSystem
{
    public static class LocalFileSystemHelper
    {
        public static IMcmaTypeRegistrations AddTypes() => McmaTypes.Add<LocalFileSystemLocator>();
    }
}