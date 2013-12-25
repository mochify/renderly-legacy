using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Net;

namespace Renderly.Utils
{
    /// <summary>
    /// This class interacts with the file system/native system to get files.
    /// </summary>
    public class RenderlyNativeFileManager : IRenderlyFileManager
    {
        public Stream Get(string path)
        {
            var uri = new Uri(path);
            var wc = new WebClient();
            return wc.OpenRead(uri);
        }

        public bool Delete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public void Fetch(string fetchPath, string savePath)
        {
            var uri = new Uri(fetchPath);
            var wc = new WebClient();
            wc.DownloadFile(fetchPath, savePath);
        }

        public void Save(Stream file, string savePath)
        {
            using(var ostream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(ostream);
            }
        }
    }
}
