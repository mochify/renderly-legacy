using System;
using System.Collections.Generic;

using System.Drawing;

namespace Renderly
{
    public class TestResult : IDisposable
    {
        private bool _disposed;

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (ReferenceImage != null)
                {
                    ReferenceImage.Dispose();
                }

                if (SourceImage != null)
                {
                    SourceImage.Dispose();
                }

                if (DifferenceImage != null)
                {
                    DifferenceImage.Dispose();
                }

                _disposed = true;
                ReferenceImage = null;
                SourceImage = null;
                DifferenceImage = null;
            }

        }
    }
}
