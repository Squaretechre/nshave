using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace NShave
{
    public class MustacheDocument
    {
        private readonly string _template;
        private readonly JObject _dataModel;
        private readonly Scope _scope;
        private readonly ScopePresentationFormat _formatting;
        private readonly StringBuilder _razorTemplate;

        public MustacheDocument(string template, JObject dataModel, Scope scope, ScopePresentationFormat formatting)
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
                string templateLine;
                while ((templateLine = reader.ReadLine()) != null)
                {
                    new MustacheControlTagLineRegexMatch(templateLine)
                        .Success(new MustacheControlTagLine(templateLine, _dataModel, _scope, _formatting).ToRazor)
                        .Fail(new MustacheVariableTagLine(templateLine, _formatting).ToRazor)
                        .Result(AppendTemplateLine);
                }
            }

            return _razorTemplate.ToString().Trim();
        }

        private void AppendTemplateLine(string line)
        {
            _razorTemplate.Append(line);
            _razorTemplate.Append(_formatting.NewLine());
        }
    }
}