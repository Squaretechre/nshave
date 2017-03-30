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
                    if (MatchIsAMustacheTagLine(match))
                    {
                        var mustacheTag = match.Groups[1].Value;
                        templateLine = new MustacheTag(mustacheTag, _dataModel, _scope, _formatting).ToRazor();
                    }
                    else
                    {
                        templateLine = new MustacheTemplateLine(templateLine, _formatting).ToRazor();
                    }

                    razorTemplate.Append(templateLine);
                    razorTemplate.Append(_formatting.NewLine());
                }
            }

            return razorTemplate.ToString().Trim();
        }

        private static bool MatchIsAMustacheTagLine(Match mustacheTag) 
            => mustacheTag.Success &&
            (mustacheTag.Groups[1].Value.First() == '#' 
            || mustacheTag.Groups[1].Value.First() == '^' 
            || mustacheTag.Groups[1].Value.First() == '/');
    }
}