using System.IO;

namespace RevitExtensions.IO
{
    /// <summary>
    /// Abstraction for file system access used by caching components.
    /// </summary>
    public interface IFileSystem
    {
        bool FileExists(string path);
        string ReadAllText(string path);
        void WriteAllText(string path, string contents);
        void CreateDirectory(string path);
    }

    internal sealed class PhysicalFileSystem : IFileSystem
    {
        public bool FileExists(string path) => File.Exists(path);
        public string ReadAllText(string path) => File.ReadAllText(path);
        public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);
        public void CreateDirectory(string path) => Directory.CreateDirectory(path);
    }
}
