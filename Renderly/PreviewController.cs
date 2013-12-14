using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Renderly
{
    class PreviewController
    {
        public PreviewController()
        {

        }

        public IEnumerable<TestResult> RunTests(IEnumerable<TestCase> testCases, DirectoryInfo reportDir)
        {
            var results = new List<TestResult>();

            var wc = new WebClient();
            foreach(var tc in testCases)
            {
                var testId = tc.TestId;
                var url = tc.Url;
                var type = tc.Type;
                var refImage = tc.ReferenceLocation;

                var result = new TestResult().ForTestId(testId);
                results.Add(result);

                byte[] imageBytes;
                try
                {
                    imageBytes = wc.DownloadData(url);
                }
                catch (Exception e)
                {
                    result.WithComment(e.Message);
                    result.Passed(false);
                    continue;
                }

                using(var ms = new MemoryStream(imageBytes))
                using(var preview = new Bitmap(ms))
                using(var convertedPreview = new Bitmap(preview.Width, preview.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                using(var reference = new Bitmap(refImage))
                {
                    using (var gr = Graphics.FromImage(convertedPreview))
                    {
                        gr.DrawImage(preview, new Rectangle(0, 0, preview.Width, preview.Height));
                    }

                    var cmp = new StandaloneImageComparator();
                    if (!cmp.Matches(reference, convertedPreview))
                    {
                        result.TestPassed = false;
                        result.WithComment("Source and Reference image do not match exactly.");
                    }
                    else
                    {
                        result.TestPassed = true;
                    }

                    using (var diffImage = cmp.GenerateDifferenceMap(reference, convertedPreview))
                    {
                        // write all the files
                        // TODO wrap each test run with a driver that writes the result files, rather
                        // than give this class the responsibility of running and saving the output images
                        // we don't want to keep the bitmap handles open because of memory limitations
                        var imgDir = reportDir.CreateSubdirectory("images");
                        var refPath = string.Format("{0}/{1}-reference.{2}", imgDir.Name, testId, Path.GetExtension(refImage));
                        var prevPath = string.Format("{0}/{1}-generated.jpg", imgDir.Name, testId);
                        var diffPath = string.Format("{0}/{1}-diff.jpg", imgDir.Name, testId);

                        reference.Save(Path.Combine(reportDir.FullName, refPath));
                        convertedPreview.Save(Path.Combine(reportDir.FullName, prevPath));
                        diffImage.Save(Path.Combine(reportDir.FullName, diffPath));
                        result.Reference = refPath;
                        result.Preview = prevPath;
                        result.Difference = diffPath;
                    }
                }
            }

            return results;
        }
    }
}
