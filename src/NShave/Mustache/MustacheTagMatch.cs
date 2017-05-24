using System;
using System.Text.RegularExpressions;

namespace NShave.Mustache
{
    public class MustacheTagMatch
    {
        private readonly string _mustacheTemplateLine;
        private string _result;

        public MustacheTagMatch(string mustacheTemplateLine)
        {
            _mustacheTemplateLine = mustacheTemplateLine;
        }

        public MustacheTagMatch ControlFlowTag(Func<string> convertControlFlowTag)
        {
            if (LineContainsMustacheControlTag(_mustacheTemplateLine) && !LineIsHtmlTag(_mustacheTemplateLine)) _result = convertControlFlowTag();
            return this;
        }

        public MustacheTagMatch InlineTag(Func<string> convertInlineTag)
        {
            if(string.IsNullOrEmpty(_result)) _result = convertInlineTag();
            return this;
        }

        private static bool LineContainsMustacheControlTag(string templateLine)
            => Regex.Match(templateLine, @"(?<![A-Za-z0-9]){{[\^|#|\/](.*)}}(?![A-Za-z0-9])", RegexOptions.IgnoreCase).Success;

	    private static bool LineIsHtmlTag(string templateLine)
		    => Regex.Match(templateLine, @"[<](.*)[>]", RegexOptions.IgnoreCase).Success;

		public void Result(Action<string> handleResult)
            => handleResult(_result);
    }
}