using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nustache.Core;

using Newtonsoft.Json;

namespace Renderly.Reporting
{
    /// <summary>
    /// This is a b
    /// </summary>
    public class MustacheView : IReportView
    {
        public MustacheView()
        {

        }

        public string GenerateTemplate(string template, IDictionary<string, object> dict)
        {
            var html = Render.StringToString(template, dict);
            return html;
        }
    }
}
