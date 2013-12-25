using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace Renderly.Utils
{
    public interface IRenderlyFileManager
    {
        Stream Get(string path);
        bool Delete(string path);
        void Fetch(string fetchPath, string savePath);
        void Save(Stream file, string savePath);
    }
}
