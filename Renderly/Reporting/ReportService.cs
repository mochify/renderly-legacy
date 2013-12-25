using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Drawing;
using System.Drawing.Imaging;

using Renderly.Utils;

namespace Renderly.Reporting
{
    public class ReportService : IReportService
    {
        IList<TestResult> _results;
        IRenderlyFileManager _assetManager;
        private readonly ReportServiceConfiguration _configuration;

        public ReportServiceConfiguration Configuration
        {
            get { return _configuration; }
        }

        public ReportService(IRenderlyFileManager assetManager, ReportServiceConfiguration config)
        {
            _assetManager = assetManager;
            _configuration = config;
            _results = new List<TestResult>();
        }


        public ReportService(IRenderlyFileManager assetManager)
            : this(assetManager, new ReportServiceConfiguration())
        {
        
        }

        /// <summary>
        /// Add a test result to use for the final report.
        /// Note that this class will not assume any ownership for
        /// images that are passed in the TestResult object.
        /// </summary>
        /// <param name="tr"></param>
        public void AddResult(TestResult tr)
        {
            if ((tr.TestPassed && Configuration.DisplaySuccesses)
                || !tr.TestPassed)
            {
                
                _results.Add(tr);
            }
        }

        /// <summary>
        /// This will save images to (somewhere). It returns an anonymous
        /// type which will tell you where the files end up.
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        private dynamic PersistAssets(TestResult tr)
        {
            var images = new List<Tuple<Image, string>>();
            
            var uuid = Guid.NewGuid().ToString("N");
            var sourcePath = string.Format("images/{0}-{1}-generated.jpg", tr.TestId, uuid);
            images.Add(Tuple.Create(tr.SourceImage, sourcePath));

            var diffPath = string.Format("images/{0}-{1}-diff.jpg", tr.TestId, uuid);
            images.Add(Tuple.Create(tr.DifferenceImage, diffPath));

            var referencePath = string.Format("images/{0}-{1}-reference.jpg", tr.TestId, uuid);

            if (Configuration.CopyReferenceImages)
            {
                images.Add(Tuple.Create(tr.ReferenceImage, referencePath));
            }
            else
            {
                referencePath = tr.OriginalReferenceLocation;
            }

            foreach (var image in images)
            {
                using(var ms = new MemoryStream())
                {
                    image.Item1.Save(ms, ImageFormat.Jpeg);
                    _assetManager.Save(ms, image.Item2);
                }
            }

            return new { Reference = referencePath, Source = sourcePath, Diff = diffPath };
        }

        private string GenerateFileName(string subdir, object identifier, object extension)
        {
            var uuid = Guid.NewGuid().ToString("N");
            return string.Format("{0}/{1}-{2}.{3}", subdir, identifier, uuid, extension);
        }

        public bool GenerateReport()
        {
            return true;
        }
    }
}
