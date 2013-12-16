using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace Renderly.Imaging
{
    public class StandaloneImageComparator : IImageComparer
    {
        public bool Matches(Bitmap reference, Bitmap compare, float threshold)
        {
            var tm = new ExhaustiveTemplateMatching(threshold);
            TemplateMatch[] matchings = tm.ProcessImage(compare, reference);
            return matchings.Any() ? true : false;
        }

        public Bitmap GenerateDifferenceMap(Bitmap reference, Bitmap compare)
        {
            var filter = new Difference(reference);
            return filter.Apply(compare);
        }
    }
}
