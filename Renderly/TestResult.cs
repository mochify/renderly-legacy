using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Renderly
{
    public class TestResult
    {
        private IList<string> _comments = new List<string>();
        public int TestId { get; set; }
        public bool TestPassed { get; set; }
        public string OriginalReferenceLocation { get; set; }
        public Image ReferenceImage { get; set; }
        public Image SourceImage { get; set; }
        public Image DifferenceImage { get; set; }
        public IEnumerable<string> Comments
        {
            get { return _comments; }
        }

        public TestResult()
        {
        }
        
        public TestResult ForTestId(int id)
        {
            TestId = id;
            return this;
        }

        public TestResult Passed(bool passed)
        {
            TestPassed = passed;
            return this;
        }

        public TestResult WithComment(string comment)
        {
            _comments.Add(comment);
            return this;
        }
    }
}
