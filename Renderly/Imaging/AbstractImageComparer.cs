using System.Drawing;

namespace Renderly.Imaging
{
    public abstract class AbstractImageComparer : IImageComparer
    {
        private float _threshold;
        public float Threshold
        {
            get { return _threshold; }
            set { _threshold = value; }
        }

        protected AbstractImageComparer(float threshold)
        {
            _threshold = threshold;
        }

        public abstract bool Matches(Bitmap reference, Bitmap compare);

        public abstract Bitmap GenerateDifferenceMap(Bitmap reference, Bitmap compare);
    }
}
