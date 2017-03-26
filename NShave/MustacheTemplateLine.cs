using System.Globalization;
using System.Text.RegularExpressions;

namespace NShave
{
    public class MustacheTemplateLine
    {
        private readonly string _templateLine;
        private readonly ScopeFormat _formatting;

        public MustacheTemplateLine(string templateLine, ScopeFormat formatting)
        {
            _templateLine = templateLine;
            _formatting = formatting;
        }

        public string ToRazor()
        {
            var razorLine = ReplaceMustacheCommentsIn(_templateLine);
            razorLine = ReplaceMustachePartialsIn(razorLine);
            razorLine = ReplaceMustacheVariablesIn(razorLine);
            razorLine = razorLine.Trim();
            return $"{_formatting.Indentation()}{razorLine}";
        }

        private static string ReplaceMustacheCommentsIn(string line)
            => Regex.Replace(line, @"{{!(.*?)}}", m =>
            {
                var comment = m.Groups[1].Value;
                return $"@*{comment}*@";
            });

        private string ReplaceMustacheVariablesIn(string line)
            => Regex.Replace(line, @"{{(.*?)}}", m =>
            {
                var propertyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(m.Groups[1].Value);
                return $"@{_formatting.ScopeNameCorrectedForRendering()}.{propertyName}";
            });

        private string ReplaceMustachePartialsIn(string line)
            => Regex.Replace(line, @"{{>(.*?)}}", m =>
            {
                var partialName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(m.Groups[1].Value).Trim();
                return $"{_formatting.ScopeMarker()}Html.Partial(\"_{partialName}\", Model)";
            });
    }
}