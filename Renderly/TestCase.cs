using System;

namespace Renderly
{
    public class TestCase
    {
        public int TestId { get; set; }
        public string Type { get; set; }
        public string SourceLocation { get; set; }
        public string ReferenceLocation { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public string Release { get; set; }
        public string Description { get; set; }
    }
}
