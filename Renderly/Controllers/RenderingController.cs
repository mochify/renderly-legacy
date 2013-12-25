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

using Renderly.Imaging;
using Renderly.Utils;

namespace Renderly.Controllers
{
    public class RenderingController
    {
        private IImageComparer _imageComparer;
        private IRenderlyAssetManager _fileManager;

        public RenderingController(IImageComparer comparer, IRenderlyAssetManager fileManager)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            _imageComparer = comparer;
            _fileManager = fileManager;
        }

        public IEnumerable<TestResult> RunTests(IEnumerable<TestCase> testCases)
        {
            foreach (var tc in testCases)
            {
                yield return RunTest(tc);
            }
        }

        private TestResult RunTest(TestCase tc)
        {
            var testId = tc.TestId;
            var sourceImage = tc.SourceLocation;
            var type = tc.Type;
            var refImage = tc.ReferenceLocation;

            var result = new TestResult().ForTestId(testId);
            result.OriginalReferenceLocation = tc.ReferenceLocation;

            using (var sourceStream = _fileManager.Get(sourceImage))
            using (var preview = new Bitmap(sourceStream))
            {
                var convertedPreview = new Bitmap(preview.Width, preview.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var reference = new Bitmap(_fileManager.Get(refImage));

                using (var gr = Graphics.FromImage(convertedPreview))
                {
                    gr.DrawImage(preview, new Rectangle(0, 0, preview.Width, preview.Height));
                }

                if (!_imageComparer.Matches(reference, convertedPreview))
                {
                    result.TestPassed = false;
                    result.WithComment("Source and Reference image do not match.");
                }
                else
                {
                    result.TestPassed = true;
                }

                var diffImage = _imageComparer.GenerateDifferenceMap(reference, convertedPreview);

                result.ReferenceImage = reference;
                result.SourceImage = convertedPreview;
                result.DifferenceImage = diffImage;
            }

            return result;
        }
    }
}
