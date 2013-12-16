using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nustache.Core;

using Newtonsoft.Json;

namespace Renderly.Views
{
    /// <summary>
    /// This is a b
    /// </summary>
    public class View
    {
        public View()
        {

        }

        public string GenerateReport(string templatePath, IDictionary<string, object> dict)
        {
            var html = Render.FileToString(templatePath, dict);
            return html;
        }
    }
}
