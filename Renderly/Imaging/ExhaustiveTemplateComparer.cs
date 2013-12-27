using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;

using Renderly.Utils;

namespace Renderly.Imaging
{
    public class ExhaustiveTemplateComparer : AbstractImageComparer
    {
        ExhaustiveTemplateMatching _templateMatcher;

        public ExhaustiveTemplateComparer(float threshold = 1.0f)
            : base(threshold)
        {
            _templateMatcher = new ExhaustiveTemplateMatching(threshold);
        }

        public override bool Matches(Bitmap reference, Bitmap compare)
        {
            Bitmap internalReference = reference.PixelFormat == PixelFormat.Format24bppRgb ?
                reference : ImageUtils.CopyBitmap(reference, PixelFormat.Format24bppRgb);

            Bitmap internalCompare = compare.PixelFormat == PixelFormat.Format24bppRgb ?
                compare : ImageUtils.CopyBitmap(compare, PixelFormat.Format24bppRgb);

            try
            {
                TemplateMatch[] matchings = _templateMatcher.ProcessImage(internalCompare, internalReference);
                return matchings.Any();
            }
            finally
            {
                // clean up our temporary images if we had to generate them.
                if (reference.PixelFormat != PixelFormat.Format24bppRgb)
                {
                    internalReference.Dispose();
                }

                if (compare.PixelFormat != PixelFormat.Format24bppRgb)
                {
                    internalCompare.Dispose();
                }
            }
        }

        public override Bitmap GenerateDifferenceMap(Bitmap reference, Bitmap compare)
        {
            Bitmap internalReference = reference.PixelFormat == PixelFormat.Format24bppRgb ?
                reference : Generate24BitImage(reference);

            Bitmap internalCompare = compare.PixelFormat == PixelFormat.Format24bppRgb ?
                compare : Generate24BitImage(compare);

            try
            {
                var filter = new Difference(internalReference);
                var diffed = filter.Apply(internalCompare);

                // let's recolor the image so that non-black pixels are colored something consistent
                var eFilter = new EuclideanColorFiltering();
                eFilter.CenterColor = new RGB(Color.Black);
                eFilter.Radius = 60;
                eFilter.FillColor = new RGB(Color.Red);
                eFilter.ApplyInPlace(diffed);

                return diffed;
            }
            finally
            {
                if (reference.PixelFormat != PixelFormat.Format24bppRgb)
                {
                    internalReference.Dispose();
                }

                if (compare.PixelFormat != PixelFormat.Format24bppRgb)
                {
                    internalCompare.Dispose();
                }
            }
        }

        private Bitmap Generate24BitImage(Bitmap img)
        {
            var newBitmap = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            newBitmap.SetResolution(img.HorizontalResolution, img.VerticalResolution);
            using (var gr = Graphics.FromImage(newBitmap))
            {
                gr.DrawImageUnscaled(img, 0, 0);
            }

            return newBitmap;
        }
    }
}
