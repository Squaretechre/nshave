using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace NShave
{
    public class MustacheDocument
    {
        private readonly string _template;
        private readonly JObject _dataModel;
        private readonly Scope _scope;
        private readonly ScopeFormat _formatting;

        public MustacheDocument(string template, JObject dataModel, Scope scope, ScopeFormat formatting)
        {
            _template = template;
            _dataModel = dataModel;
            _scope = scope;
            _formatting = formatting;
        }

        public string ToRazor()
        {
            var razorTemplate = new StringBuilder();
            using (var reader = new StringReader(_template))
            {
                string templateLine;
                while ((templateLine = reader.ReadLine()) != null)
                {
                    var match = Regex.Match(templateLine, @"{{(.*)}}", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        var mustacheTag = match.Groups[1].Value;

                        switch (mustacheTag.First())
                        {
                            case '#':
                            case '^':
                            case '/':
                                templateLine = new MustacheTag(mustacheTag, _dataModel, _scope, _formatting).ToRazor();
                                break;
                            default:
                                templateLine = new MustacheTemplateLine(templateLine, _formatting).ToRazor();
                                break;
                        }
                    }

                    razorTemplate.Append(templateLine);
                    razorTemplate.Append(_formatting.NewLine());
                }
            }

            return razorTemplate.ToString().Trim();
        }
    }
}