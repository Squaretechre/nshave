using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NShave.Tests
{
    [TestFixture]
    public class NShaveTests
    {
        public const string DataTemplate =
            @"{
  ""header"": ""Colors"",
  ""items"": [
      {""name"": ""red"", ""first"": true, ""url"": ""#Red""},
      {""name"": ""green"", ""link"": true, ""url"": ""#Green""},
      {""name"": ""blue"", ""link"": true, ""url"": ""#Blue""}
  ],
  ""empty"": false
}";

        [TestCase]
        public void ConvertMustacheBooleanStatementToRazorIfStatement()
        {
            const string mustache = 
@"{{#empty}}
    <p>hello, world!</p>
{{/empty}}";

            const string razor = 
@"if (@Model.empty) {
    <p>hello, world!</p>
}";

            var dataModel = (JObject) JsonConvert.DeserializeObject(DataTemplate);

            var convertedMustache = MustacheToRazor(mustache, dataModel);
            Assert.That(convertedMustache, Is.EqualTo(razor));
        }

        private static string MustacheToRazor(string mustacheTemplate, JObject dataModel)
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
                        var propertyName = mustacheToken.Substring(1, mustacheToken.Length - 1);

                        if (mustacheToken.First() == '#')
                        {
                            var propertyType = dataModel[propertyName].Type;

                            switch (propertyType)
                            {
                                case JTokenType.Boolean:
                                    line = $"if (@Model.{propertyName}) {{";
                                    break;
                            }
                        }
                        else if (mustacheToken.First() == '/')
                        {
                            line = "}";
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