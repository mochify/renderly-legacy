using System.Drawing;

namespace Renderly.Imaging
{
    /// <summary>
    /// An abstract class on top of IImageComparer which adds the Threshold state.
    /// </summary>
    public abstract class AbstractImageComparer : IImageComparer
    {
        private float _threshold;

        /// <summary>
        /// A metric for defining how to fail/reject images for matches.
        /// What this means depends on the comparison algorithm.
        /// </summary>
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
