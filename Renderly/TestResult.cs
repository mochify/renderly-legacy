using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renderly
{
    class TestResult
    {
        private IList<string> _comments = new List<string>();
        public int TestId { get; set; }
        public bool TestPassed { get; set; }
        public string Reference { get; set; }
        public string Preview { get; set; }
        public string Difference { get; set; }
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
