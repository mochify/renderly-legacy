using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge.Imaging;

namespace Renderly
{
    class PreviewController
    {
        public PreviewController()
        {

        }

        public void RunTests(IEnumerable<TestCase> testCases)
        {
            var wc = new WebClient();
            foreach(var tc in testCases)
            {
                var testId = tc.TestId;
                var url = tc.Url;
                var type = tc.Type;
                var refImage = tc.ReferenceLocation;

                byte[] imageBytes = wc.DownloadData(url);
                MemoryStream ms = new MemoryStream(imageBytes);
                ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
                //tm.ProcessImage()

                var preview = new Bitmap(System.Drawing.Image.FromStream(ms));
                var convertedPreview = new Bitmap(preview.Width, preview.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                using (var gr = Graphics.FromImage(convertedPreview))
                {
                    gr.DrawImage(preview, new Rectangle(0, 0, preview.Width, preview.Height));
                }

                var reference = new Bitmap(refImage);
                Console.WriteLine("Reference pixelformat {0}", reference.PixelFormat);




                TemplateMatch[] matchings = tm.ProcessImage(reference, convertedPreview);

                Console.WriteLine(matchings[0].Similarity);
                
            }
        }
    }
}
