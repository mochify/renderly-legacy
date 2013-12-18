using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Net;
using Microsoft.Win32;

namespace Renderly.Utils
{
    public class TestCaseGenerator
    {
        public void Run(int testId, string url, string type, string referenceDir)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;

            var rsp = request.GetResponse();

            // TODO make this support all the types that could be rendered...which are just jpg/gif/png, so whatever
            using(var rspStream = rsp.GetResponseStream())
            using(var fstream = File.OpenWrite(Path.Combine(referenceDir, string.Format("{0}.jpg", testId))))
            {
                rspStream.CopyToAsync(fstream);
            }
        }
    }
}
