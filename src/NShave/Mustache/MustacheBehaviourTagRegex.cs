using System;
using System.Text.RegularExpressions;

namespace NShave.Mustache
{
    public class MustacheBehaviourTagRegex
    {
        private readonly string _mustacheTemplateLine;
        private string _result;

        public MustacheBehaviourTagRegex(string mustacheTemplateLine)
        {
            _mustacheTemplateLine = mustacheTemplateLine;
        }

        public MustacheBehaviourTagRegex Match(Func<string> success)
        {
            if (LineContainsControlFlowTag(_mustacheTemplateLine)) _result = success();
            return this;
        }

        public MustacheBehaviourTagRegex NoMatch(Func<string> fail)
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