using System.Drawing;

namespace Renderly.Imaging
{
    /// <summary>
    /// This is the interface to specify image comparison methods that determine
    /// if images match, and to generate the differences between two images.
    /// </summary>
    public interface IImageComparer
    {
        /// <summary>
        /// This method determines if two images are a match, based on a threshold.
        /// </summary>
        /// <param name="reference">The reference/base image to use for comparison. Should be a 24 bit RGB bitmap.</param>
        /// <param name="compare">The image to compare the reference against. Should be a 24 bit RGB bitmap.</param>
        /// <returns></returns>
        bool Matches(Bitmap reference, Bitmap compare);

        /// <summary>
        /// Generates an image that shows the pixel differences between a reference and comparison image.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="compare"></param>
        /// <returns>A System.Drawing.Bitmap that shows the pixel differences between reference and compare (e.g. how compare looks different from reference). The Bitmap's ownership is passed to whoever calls this.</returns>
        Bitmap GenerateDifferenceMap(Bitmap reference, Bitmap compare);
    }
}
