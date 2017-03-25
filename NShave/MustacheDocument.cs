using System;
using System.Globalization;
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

        public MustacheDocument(string template, JObject dataModel)
        {
            _template = template;
            _dataModel = dataModel;
        }

        public string ToRazor()
        {
            var razorTemplate = new StringBuilder();
            using (var reader = new StringReader(_template))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var match = Regex.Match(line, @"{{(.*)}}", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        var mustacheTag = match.Groups[1].Value;

                        switch (mustacheTag.First())
                        {
                            case '#':
                            case '^':
                                line = new MustacheTag(mustacheTag, _dataModel).ToRazor();
                                break;
                            case '/':
                                line = "}";
                                break;
                            default:
                                line = Regex.Replace(line, @"{{(.*?)}}", m =>
                                {
                                    var propertyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(m.Groups[1].Value);
                                    return $"@Model.{propertyName}";
                                });
                                break;
                        }
                    }

                    razorTemplate.Append(line);
                    razorTemplate.Append(Environment.NewLine);
                }
            }

            razorTemplate.Length = razorTemplate.Length - 2;
            return razorTemplate.ToString();
        }
    }
}