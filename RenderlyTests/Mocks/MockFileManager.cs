using System.Collections.Generic;
using System.IO;

using Renderly.Utils;

namespace RenderlyTests.Mocks
{
    public class MockFileManager : IRenderlyAssetManager
    {
        private Dictionary<string, byte[]> _pretendFileSystem;

        internal MockFileManager()
        {
            _pretendFileSystem = new Dictionary<string, byte[]>();
        }

        public Stream Get(string path)
        {
            byte[] ret;
            bool found = _pretendFileSystem.TryGetValue(path, out ret);
            return found ? new MemoryStream(ret) : null;
        }

        public bool Delete(string path)
        {
            _pretendFileSystem.Remove(path);
            return true;
        }

        public void Fetch(string fetchPath, string savePath)
        {

        }

        public string FetchToRandomFilename(string uri, string directory)
        {
            return "";
        }

        public void Save(Stream file, string savePath)
        {
            file.Position = 0;
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                _pretendFileSystem.Add(savePath, memoryStream.GetBuffer());
            }
        }

        public void Save(string file, string savePath)
        {
            return;
        }

        public void CreateFolder(string folderPath)
        {
            return;
        }
    }
}
