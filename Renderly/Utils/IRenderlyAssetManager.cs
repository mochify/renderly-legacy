using System.IO;

namespace Renderly.Utils
{
    public interface IRenderlyAssetManager
    {
        Stream Get(string path);
        bool Delete(string path);
        void Fetch(string fetchPath, string savePath);
        void Save(Stream file, string savePath);

        void Save(string contents, string savePath);

        string FetchToRandomFilename(string fetchUri, string saveDirectory);

        /// <summary>
        /// Create a folder, and all paths in the path that don't exist.
        /// </summary>
        /// <param name="path"></param>
        void CreateFolder(string path);
    }
}
