using System.Collections.Generic;
using RevitExtensions;

namespace RevitExtensions.Tests
{
    internal sealed class InMemoryFileSystem : IFileSystem
    {
        private readonly Dictionary<string, string> _files = new Dictionary<string, string>();

        public bool FileExists(string path) => _files.ContainsKey(path);

        public string ReadAllText(string path) => _files[path];

        public void WriteAllText(string path, string contents) => _files[path] = contents;

        public void CreateDirectory(string path) { }
    }
}
