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
    public class ExhaustiveTemplateComparer : AbstractImageComparer
    {
        public ExhaustiveTemplateComparer(float threshold = 1.0f)
            : base(threshold)
        {
        }

        public override bool Matches(Bitmap reference, Bitmap compare)
        {
            var tm = new ExhaustiveTemplateMatching(Threshold);
            TemplateMatch[] matchings = tm.ProcessImage(compare, reference);
            return matchings.Any() ? true : false;
        }

        public override Bitmap GenerateDifferenceMap(Bitmap reference, Bitmap compare)
        {
            var filter = new Difference(reference);
            return filter.Apply(compare);
        }
    }
}
