using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace NShave
{
    public static class TextInfoExtensions
    {
        public static string ToTitleCase(this TextInfo textInfo, string input) 
            => input
            .Split('.')
            .Select(FirstLetterToUpperCase)
            .Aggregate(JoinWordList());

        private static Func<string, string, string> JoinWordList() 
            => (current, next) => $"{current}.{next}";

        private static string FirstLetterToUpperCase(string input)
        {
            var match = Regex.Match(input, @"[a-zA-Z]");
            if (!match.Success) return input;
            var upperCased = input;
            var firstAlphaChar = match.Groups[0].Value;
            var firstAlphaCharPosition = upperCased.IndexOf(firstAlphaChar, StringComparison.Ordinal);
            upperCased = upperCased.Remove(firstAlphaCharPosition, 1);
            upperCased = upperCased.Insert(firstAlphaCharPosition, firstAlphaChar.ToUpper());
            return upperCased;
        }
    }
}