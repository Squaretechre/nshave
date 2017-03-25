using System.Globalization;
using System.Text.RegularExpressions;

namespace NShave
{
    public class MustacheTemplateLine
    {
        private readonly string _templateLine;

        public MustacheTemplateLine(string templateLine)
        {
            _templateLine = templateLine;
        }

        public string ToRazor()
        {
            return ReplaceMustacheVariablesIn(_templateLine);
        }

        private static string ReplaceMustacheVariablesIn(string line)
            => Regex.Replace(line, @"{{(.*?)}}", m =>
            {
                var propertyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(m.Groups[1].Value);
                return $"@Model.{propertyName}";
            });
    }
}