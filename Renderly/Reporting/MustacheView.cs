using System.Collections.Generic;

using Nustache.Core;

namespace Renderly.Reporting
{
    /// <summary>
    /// This is an implementation of IReportView that uses the
    /// <a href="http://mustache.github.io/">Mustache templating language</a>.
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
