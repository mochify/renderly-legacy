using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renderly.Reporting
{
    /// <summary>
    /// This is the base interface for the various views that can be used
    /// to generate the resulting report.
    /// </summary>
    public interface IReportView
    {
        string GenerateTemplate(string template, IDictionary<string, object> mapper);
    }
}
