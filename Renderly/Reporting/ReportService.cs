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
        /// <summary>
        /// A lightweight class to hold basically the same stuff as Renderly.TestResult,
        /// but without the image references. 
        /// </summary>
        class ReportResult
        {
            private IList<string> _comments = new List<string>();
            public int TestId { get; set; }
            public bool TestPassed { get; set; }
            public string ReferencePath { get; set; }
            public string SourcePath { get; set; }
            public string DifferencePath { get; set; }

            public IList<string> Comments
            {
                get { return _comments; }
                set { _comments = value; }
            }
        }

        IList<ReportResult> _results;
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
            _results = new List<ReportResult>();
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
                dynamic locations = PersistAssets(tr);
                var reportResult = new ReportResult
                {
                    TestId = tr.TestId,
                    TestPassed = tr.TestPassed,
                    Comments = new List<string>(tr.Comments),
                    SourcePath = MakeUri(locations.Source),
                    ReferencePath = MakeUri(locations.Reference),
                    DifferencePath = MakeUri(locations.Difference)
                };
                _results.Add(reportResult);
            }
        }

        private string MakeUri(string path)
        {
            Uri uri;
            bool created = Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out uri);

            if (created)
            {
                return uri.ToString();
            }
            return "";
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
            var sourcePath = string.Format("images/{1}-{2}-generated.jpg", Configuration.ReportName, tr.TestId, uuid);
            images.Add(Tuple.Create(tr.SourceImage, sourcePath));

            var diffPath = string.Format("images/{1}-{2}-diff.jpg", Configuration.ReportName, tr.TestId, uuid);
            images.Add(Tuple.Create(tr.DifferenceImage, diffPath));

            var referencePath = string.Format("images/{1}-{2}-reference.jpg", Configuration.ReportName, tr.TestId, uuid);

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
                image.Item1.Save(Path.Combine(Configuration.OutputDirectory, Configuration.ReportName, image.Item2));
                //using (var ms = new MemoryStream())
                //{
                //    image.Item1.Save(ms, ImageFormat.Jpeg);
                //    _assetManager.Save(ms, string.Join("/", Configuration.OutputDirectory, image.Item2));
                //}
            }

            // yes I am evil - returning an anonymous type via dynamic.
            // This is localized to just inside this class, and I did tradeoff typesafety/autocomplete
            // for ease of coding, since otherwise I'd have to make a new class
            // or return a nameless Tuple
            return new { Reference = referencePath, Source = sourcePath, Difference = diffPath };
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
                var outFile = Path.Combine(Configuration.OutputDirectory, Configuration.ReportName, "report.html");
                _assetManager.Save(output, outFile);
            }

            return true;
        }
    }
}
