using System;
using System.Globalization;
using System.Text;
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
            var firstNonWhiteSpaceChar = Regex.Match(_templateLine, @"[^\s\\]").Groups[0].Value;
            var indexOfFirstNonWhiteSpaceChar = _templateLine.IndexOf(firstNonWhiteSpaceChar, StringComparison.Ordinal);
            var leadingWhiteSpace = _templateLine.Substring(0, indexOfFirstNonWhiteSpaceChar);
	        var razorLine = ReplaceInlineIfStatements(_templateLine);
			razorLine = ReplaceMustacheCommentsIn(razorLine);
            razorLine = ReplaceMustacheWildcardVariablesIn(razorLine);
            razorLine = ReplaceMustachePartialsIn(razorLine);
            razorLine = ReplaceMustacheVariablesIn(razorLine);
            razorLine = razorLine.Trim();
            return $"{leadingWhiteSpace}{razorLine}";
        }

        private void UpdateScopeForLine(string templateLine)
        {
            var enterListMatch = Regex.Match(templateLine, @"<ul>", RegexOptions.IgnoreCase);
            var exitListMatch = Regex.Match(templateLine, @"</ul>", RegexOptions.IgnoreCase);
            if (enterListMatch.Success) _formatting.Enter(new ScopeType("ul", TokenType.HtmlUnorderedList));
            if (exitListMatch.Success) _formatting.Leave("ul");
        }

	    private string ReplaceInlineIfStatements(string line)
	    {
		    var itemRegex = new Regex(@"{{[\^|#|\/](.*)}}");
		    var matches = itemRegex.Matches(line);
		    foreach (Match match in matches)
		    {
			    var ifStatementLine = match.Groups[0].Value;
			    var predicateRegex = new Regex(@"(?!{{#)(.*?) (?<!}})");
			    var bodyRegex = new Regex(@"(?<=\}}).+?(?={{\/)");

			    var predicate = predicateRegex.Matches(ifStatementLine)[1].Value.Trim();
			    var body = bodyRegex.Match(ifStatementLine).Value.Trim();

			    predicate = RemoveScopeMarker(RazorifyVariable(predicate));
				body = RemoveScopeMarker(ReplaceMustacheVariablesIn(body));

			    var stringifyedBody = StringFormatifyBody(body);
			    var razorInlineIfStatement = CreateInlineRazorIfStatement(predicate, stringifyedBody);

				line = Regex.Replace(line, @"{{[\^|#|\/](.*)}}", m => razorInlineIfStatement);
			}
		    return line;
	    }

	    private static string StringFormatifyBody(string body)
	    {
		    var parts = body.Split('=');
		    var leftHandOfAssignment = parts[0];
		    var rightHandOfAssignment = parts[1].Replace("\"", "");
			var stringBuilder = new StringBuilder();
		    stringBuilder.Append(@"string.Format(""");
		    stringBuilder.Append(leftHandOfAssignment);
			stringBuilder.Append("={0}\", ");
		    stringBuilder.Append(rightHandOfAssignment);
		    stringBuilder.Append(")");
		    var stringFormatifyBody = stringBuilder.ToString();
		    return stringFormatifyBody;
	    }

	    private static string RemoveScopeMarker(string razorLine)
		    => razorLine.Replace("@", "");

	    private static string RazorifyVariable(string predicate)
		    => $"@Model.{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(predicate.Trim())}";

		private static string CreateInlineRazorIfStatement(string predicate, string stringifyedBody)
		    => $"@(!string.IsNullOrEmpty({predicate}) ? {stringifyedBody} : string.Empty)";

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
                return $"@{_formatting.ScopeNameCorrectedForRendering()}.{variableName}".Replace(" ", "");
            });

        private static string ReplaceMustachePartialsIn(string line)
            => Regex.Replace(line, @"{{>(.*?)}}", match =>
            {
                var partialName = VariableNameFromRegexMatch(match).Trim();
                return ($"@Html.Partial(\"_{partialName}\", (object)Model)");
            });

        private string ReplaceMustacheWildcardVariablesIn(string line)
            => Regex.Replace(line, @"{{.}}", match => $"@{_formatting.ScopeNameCorrectedForRendering()}");

        private static string VariableNameFromRegexMatch(Match match)
            => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(match.Groups[1].Value);
    }
}