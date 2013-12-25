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
        IRenderlyAssetManager _assetManager;
        private readonly ReportServiceConfiguration _configuration;

        public ReportServiceConfiguration Configuration
        {
            get { return _configuration; }
        }

        public ReportService(IRenderlyAssetManager assetManager, ReportServiceConfiguration config)
        {
            _assetManager = assetManager;
            _configuration = config;
            _results = new List<TestResult>();
        }


        public ReportService(IRenderlyAssetManager assetManager)
            : this(assetManager, new ReportServiceConfiguration())
        {
        
        }

        /// <summary>
        /// Add a test result to use for the final report.
        /// Note that passing in a TestResult does not transfer ownership
        /// of the internal bitmaps to me.
        /// </summary>
        /// <param name="tr"></param>
        public void AddResult(TestResult tr)
        {
            if ((tr.TestPassed && Configuration.DisplaySuccesses)
                || !tr.TestPassed)
            {
                _results.Add(tr);
            }
            PersistAssets(tr);
        }

        private void CreateReportLayout()
        {
            _assetManager.CreateFolder(string.Join("/", Configuration.OutputDirectory,
                Configuration.ReportName, "images"));
        }

        /// <summary>
        /// This will save images to (somewhere). It returns an anonymous
        /// type which will tell you where the files end up.
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        private dynamic PersistAssets(TestResult tr)
        {
            CreateReportLayout();

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
                using (var ms = new MemoryStream())
                {
                    image.Item1.Save(ms, ImageFormat.Jpeg);
                    _assetManager.Save(ms, string.Join("/", Configuration.OutputDirectory, image.Item2));
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
            var defaultTemplate = "template.rend";
            using (var s = _assetManager.Get(Path.Combine(Configuration.TemplateDirectory, defaultTemplate)))
            using (var sr = new StreamReader(s))
            {
                var reportDict = new Dictionary<string, object>();
                reportDict.Add("reportname", Configuration.ReportName);
                reportDict.Add("result", _results);
                var template = sr.ReadToEnd();
                var output = Configuration.ReportView.GenerateTemplate(template, reportDict);
                var outFile = Path.Combine(Configuration.OutputDirectory, "report.html");
                _assetManager.Save(output, outFile);
            }

            return true;
        }
    }
}
