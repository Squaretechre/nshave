using System.Globalization;
using System.Text.RegularExpressions;
using NShave.Extensions;
using NShave.Scope;

namespace NShave.Mustache
{
    public class MustacheInlineTagLine
    {
        private readonly ScopePresentation _formatting;
        private readonly string _templateLine;

        public MustacheInlineTagLine(string templateLine, ScopePresentation formatting)
        {
            _templateLine = templateLine;
            _formatting = formatting;
        }

        public string ToRazor()
        {
            UpdateScopeForLine(_templateLine);
            var razorLine = ReplaceMustacheCommentsIn(_templateLine);
            razorLine = ReplaceMustacheWildcardVariablesIn(razorLine);
            razorLine = ReplaceMustachePartialsIn(razorLine);
            razorLine = ReplaceMustacheVariablesIn(razorLine);
            razorLine = razorLine.Trim();
            return $"{_formatting.Indentation()}{razorLine}";
        }

        private void UpdateScopeForLine(string templateLine)
        {
            var enterListMatch = Regex.Match(templateLine, @"<ul>", RegexOptions.IgnoreCase);
            var exitListMatch = Regex.Match(templateLine, @"</ul>", RegexOptions.IgnoreCase);
            if (enterListMatch.Success) _formatting.Enter(new ScopeType("ul", TokenType.HtmlUnorderedList));
            if (exitListMatch.Success) _formatting.Leave("ul");
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
                return _formatting.ApplyScopeMarker($"Html.Partial(\"_{partialName}\", (object)Model)");
            });

        private string ReplaceMustacheWildcardVariablesIn(string line)
            => Regex.Replace(line, @"{{.}}", match => $"@{_formatting.ScopeNameCorrectedForRendering()}");

        private static string VariableNameFromRegexMatch(Match match)
            => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(match.Groups[1].Value);
    }
}