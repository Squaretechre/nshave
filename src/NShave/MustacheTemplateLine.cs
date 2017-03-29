using System.Globalization;
using System.Text.RegularExpressions;

namespace NShave
{
    public class MustacheTemplateLine
    {
        private readonly ScopeFormat _formatting;
        private readonly string _templateLine;

        public MustacheTemplateLine(string templateLine, ScopeFormat formatting)
        {
            _templateLine = templateLine;
            _formatting = formatting;
        }

        public string ToRazor()
        {
            var razorLine = ReplaceMustacheCommentsIn(_templateLine);
            razorLine = ReplaceMustacheWildcardVariablesIn(razorLine);
            razorLine = ReplaceMustachePartialsIn(razorLine);
            razorLine = ReplaceMustacheVariablesIn(razorLine);
            razorLine = razorLine.Trim();
            return $"{_formatting.Indentation()}{razorLine}";
        }

        private static string ReplaceMustacheCommentsIn(string line)
            => Regex.Replace(line, @"{{!(.*?)}}", match =>
            {
                var comment = match.Groups[1].Value;
                return $"@*{comment}*@";
            });

        private string ReplaceMustacheVariablesIn(string line)
            => Regex.Replace(line, @"{{(.*?)}}", match =>
            {
                var variableName = VariableNameFromRegexMatch(match);
                return $"@{_formatting.ScopeNameCorrectedForRendering()}.{variableName}";
            });

        private string ReplaceMustachePartialsIn(string line)
            => Regex.Replace(line, @"{{>(.*?)}}", match =>
            {
                var partialName = VariableNameFromRegexMatch(match).Trim();
                return $"{_formatting.ScopeMarker()}Html.Partial(\"_{partialName}\", Model)";
            });

        private string ReplaceMustacheWildcardVariablesIn(string line)
            => Regex.Replace(line, @"{{.}}", match => $"@{_formatting.ScopeNameCorrectedForRendering()}");

        private static string VariableNameFromRegexMatch(Match match)
            => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(match.Groups[1].Value);
    }
}