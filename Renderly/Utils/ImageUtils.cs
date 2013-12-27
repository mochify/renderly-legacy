
using System.Drawing;
using System.Drawing.Imaging;

namespace Renderly.Utils
{
    public static class ImageUtils
    {
        public static Bitmap CopyBitmap(Bitmap bmp, PixelFormat targetFormat)
        {
            var retBmp = new Bitmap(bmp.Width, bmp.Height, targetFormat);
            retBmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
            using (var gr = Graphics.FromImage(retBmp))
            {
                gr.DrawImageUnscaled(bmp, Point.Empty);
                return retBmp;
            }
        }
    }
}
