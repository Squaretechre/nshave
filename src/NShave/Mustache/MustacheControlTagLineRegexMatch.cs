using System;
using System.Text.RegularExpressions;

namespace NShave.Mustache
{
    public class MustacheControlTagLineRegexMatch
    {
        private readonly string _mustacheTemplateLine;
        private string _result;

        public MustacheControlTagLineRegexMatch(string mustacheTemplateLine)
        {
            _mustacheTemplateLine = mustacheTemplateLine;
        }

        public MustacheControlTagLineRegexMatch Success(Func<string> success)
        {
            if (LineContainsControlFlowTag(_mustacheTemplateLine)) _result = success();
            return this;
        }

        public MustacheControlTagLineRegexMatch Fail(Func<string> fail)
        {
            if (!LineContainsControlFlowTag(_mustacheTemplateLine)) _result = fail();
            return this;
        }

        private static bool LineContainsControlFlowTag(string templateLine)
            => Regex.Match(templateLine, @"{{[\^|#|\/](.*)}}", RegexOptions.IgnoreCase).Success;

        public void Result(Action<string> handleResult)
            => handleResult(_result);
    }
}