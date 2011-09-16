using System;
using System.IO;
namespace Microsoft.Xna.Framework.Storage
{
    public class StorageContainer : IDisposable
    {
        string path;

        public StorageContainer(string name)
        {
            this.path = string.Format("SavedGames/{0}", name);
            Directory.CreateDirectory(path);
        }

        public void Dispose()
        {
        }

        public Stream OpenFile(string fileName, FileMode mode, FileAccess access)
        {
            return File.Open(string.Format("{0}/{1}", path, fileName), mode, access);
        }
    }
}

