﻿using System;
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
        public void ConvertMustacheIfStatementToRazorIfStatement()
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

        [TestCase]
        public void ConvertMustacheLoopToRazorLoop()
        {
            const string mustache =
@"{{#items}}
    <p>hello, world!</p>
{{/items}}";

            const string razor =
@"foreach (var item in Model.items) {
    <p>hello, world!</p>
}";

            var dataModel = (JObject)JsonConvert.DeserializeObject(DataTemplate);

            var convertedMustache = MustacheToRazor(mustache, dataModel);
            Assert.That(convertedMustache, Is.EqualTo(razor));
        }

        [TestCase]
        public void ConvertMustacheModelValueWithRazorModelValue()
        {
            const string mustache = @"<h1>{{header}}</h1>";
            const string razor = @"<h1>@Model.header</h1>";

            var dataModel = (JObject)JsonConvert.DeserializeObject(DataTemplate);

            var convertedMustache = MustacheToRazor(mustache, dataModel);
            Assert.That(convertedMustache, Is.EqualTo(razor));
        }

        [TestCase]
        public void ConvertMustacheLineWithMultipleModelValuesToRazor()
        {
            const string mustache = @"<p><span>{{header}}</span><span>{{header}}</span></p>";
            const string razor = @"<p><span>@Model.header</span><span>@Model.header</span></p>";

            var dataModel = (JObject)JsonConvert.DeserializeObject(DataTemplate);

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

                        switch (mustacheToken.First())
                        {
                            case '#':
                                var propertyType = dataModel[propertyName].Type;

                                switch (propertyType)
                                {
                                    case JTokenType.Boolean:
                                        line = $"if (@Model.{propertyName}) {{";
                                        break;
                                    case JTokenType.Array:
                                        var singularName = propertyName.Substring(0, propertyName.Length - 1);
                                        line = $"foreach (var {singularName} in Model.{propertyName}) {{";
                                        break;
                                }
                                break;
                            case '/':
                                line = "}";
                                break;
                            default:
                                line = Regex.Replace(line, @"{{(.*?)}}", m => $"@Model.{m.Groups[1].Value}");
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