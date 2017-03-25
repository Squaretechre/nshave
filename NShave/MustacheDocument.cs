using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace NShave
{
    public class MustacheDocument
    {
        private static string _template;
        private static JObject _dataModel;
        private readonly Scope _scope;

        public MustacheDocument(string template, JObject dataModel, Scope scope)
        {
            _template = template;
            _dataModel = dataModel;
            _scope = scope;
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
                                templateLine = new MustacheTag(mustacheTag, _dataModel, _scope).ToRazor();
                                break;
                            default:
                                templateLine = new MustacheTemplateLine(templateLine).ToRazor();
                                break;
                        }
                    }

                    razorTemplate.Append(templateLine);
                    razorTemplate.Append(Environment.NewLine);
                }
            }

            RemoveTrailingNewLineFrom(razorTemplate);
            return razorTemplate.ToString();
        }

        private static void RemoveTrailingNewLineFrom(StringBuilder razorTemplate)
        {
            razorTemplate.Length = razorTemplate.Length - 2;
        }
    }
}