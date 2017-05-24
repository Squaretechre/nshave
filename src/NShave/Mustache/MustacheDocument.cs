using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using NShave.Scope;

namespace NShave.Mustache
{
    public class MustacheDocument
    {
        private readonly string _template;
        private readonly JObject _dataModel;
        private readonly ScopeDataModel _scope;
        private readonly ScopePresentation _formatting;
        private readonly StringBuilder _razorTemplate;

        public MustacheDocument(string template, JObject dataModel, ScopeDataModel scope, ScopePresentation formatting)
        {
            _template = template;
            _dataModel = dataModel;
            _scope = scope;
            _formatting = formatting;
            _razorTemplate = new StringBuilder();
        }

        public string ToRazor()
        {
            using (var reader = new StringReader(_template))
            {
                string mustacheLine;
                while ((mustacheLine = reader.ReadLine()) != null)
                {
                    new MustacheTagMatch(mustacheLine)
                        .ControlFlowTag(new MustacheBehaviourTagLine(mustacheLine, _dataModel, _scope, _formatting).ToRazor)
                        .InlineTag(new MustacheInlineTagLine(mustacheLine, _formatting).ToRazor)
                        .Result(AppendRazorLine);
                }
            }

            return _razorTemplate.ToString().Trim();
        }

        private void AppendRazorLine(string line)
        {
            _razorTemplate.Append(line);
            _razorTemplate.Append(_formatting.NewLine());
        }
    }
}