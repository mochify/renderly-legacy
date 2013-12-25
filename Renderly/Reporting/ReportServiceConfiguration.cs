using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renderly.Reporting
{
    public class ReportServiceConfiguration
    {
        private bool _copyReferenceImages;
        private bool _reportSuccesses;
        private string _outputPath;

        public bool CopyReferenceImages
        {
            get { return _copyReferenceImages; }
            set { _copyReferenceImages = value; }
        }

        public bool DisplaySuccesses
        {
            get { return _reportSuccesses; }
            set { _reportSuccesses = value; }
        }

        public string OutputDirectory
        {
            get { return _outputPath; }
            set { _outputPath = value; }
        }
    }
}
