using System.IO;

namespace Renderly.Utils
{
    /// <summary>
    /// This is an interface that defines the asset managers used for Renderly.
    /// 
    /// An asset manager is a class that handles CRUD operations on stuff beyond the application.
    /// This includes things like files and anything that involves I/O beyond the program.
    /// 
    /// The goal of the asset manager is to abstract away concepts like the filesystem and
    /// the network, because:
    /// 
    /// * It simplifies testing (can mock this class out or just use an implementation that doesn't use the filesystem)
    /// * You may not want to be tied to an actual filesystem. The provided implementations of classes that require assets (like ReportService and RenderingController)
    ///   do not rely on any specific location for their assets.
    /// </summary>
    public interface IRenderlyAssetManager
    {
        /// <summary>
        /// Retrieve a stream to an asset/object with a valid URI.
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="System.IO.IOException">Thrown if object is not found or an error was encountered retrieving it.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the provided path is not a valid URI.</exception>
        /// <returns></returns>
        Stream Get(string path);

        /// <summary>
        /// Deletes an object.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool Delete(string path);

        /// <summary>
        /// Retrieves an object from a resource and saves it to a provided path (including filename)
        /// </summary>
        /// <param name="fetchPath"></param>
        /// <param name="savePath"></param>
        void Fetch(string fetchPath, string savePath);

        /// <summary>
        /// Saves a stream to a provided path (including filename)
        /// </summary>
        /// <param name="file"></param>
        /// <param name="savePath"></param>
        void Save(Stream file, string savePath);

        void Save(string contents, string savePath);

        /// <summary>
        /// Retrieves an object from a resource and saves it with a random name to a provided directory.
        /// 
        /// </summary>
        /// <param name="fetchUri">The URI of the resource to retrieve.</param>
        /// <param name="saveDirectory">The directory to save the object in.</param>
        /// <returns>The generated name of the object.</returns>
        string FetchToRandomFilename(string fetchUri, string saveDirectory);

        /// <summary>
        /// Create a folder, and all paths in the path that don't exist.
        /// </summary>
        /// <param name="path"></param>
        void CreateFolder(string path);
    }
}
