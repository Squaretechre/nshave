using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NShave.Tests
{
    [TestFixture]
    public class NShaveTests
    {
        [TestCase]
        public void ConvertMustacheIfStatementToRazorIfStatement()
        {
            const string mustache = 
@"{{#items}}
    <p>hello, world!</p>
{{/items}}";

            const string razor = 
@"if (@Model.items) {
    <p>hello, world!</p>
}";

            var convertedMustache = MustacheToRazor(mustache);
            Assert.That(convertedMustache, Is.EqualTo(razor));
        }

        private static string MustacheToRazor(string mustacheTemplate)
        {
            var razorTemplate = new StringBuilder();
            using (var reader = new StringReader(mustacheTemplate))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var match = Regex.Match(line, @"{{(.*)}}", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        var mustacheToken = match.Groups[1].Value;

                        switch (mustacheToken.First())
                        {
                            case '#':
                                var propertyName = mustacheToken.Substring(1, mustacheToken.Length - 1);
                                line = $"if (@Model.{propertyName}) {{";
                                break;
                            case '/':
                                line = "}";
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