using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NShave.Tests
{
    [TestFixture]
    public class NShaveTests
    {
        public const string DataTemplateColors =
@"{
  ""header"": ""Colors"",
  ""items"": [
      {""name"": ""red"", ""first"": true, ""url"": ""#Red""},
      {""name"": ""green"", ""link"": true, ""url"": ""#Green""},
      {""name"": ""blue"", ""link"": true, ""url"": ""#Blue""}
  ],
  ""empty"": false
}";

        public const string DataTemplatePerson = 
@"{
  ""name"": {
    ""first"": ""John"",
    ""last"": ""Doe""
  }
}";

        [TestCase]
        public void ConvertMustacheTruthyIfStatementToRazor()
        {
            const string mustache =
@"{{#empty}}
    <p>hello, world!</p>
{{/empty}}";

            const string expectedRazor =
@"@if (Model.Empty) {
    <p>hello, world!</p>
}";

            AssertCorrectConversion(mustache, expectedRazor, DataTemplateColors);
        }

        [TestCase]
        public void ConvertMustacheFalseyIfStatementToRazor()
        {
            const string mustache =
@"{{^empty}}
    <p>hello, world!</p>
{{/empty}}";

            const string expectedRazor =
@"@if (!Model.Empty) {
    <p>hello, world!</p>
}";

            AssertCorrectConversion(mustache, expectedRazor, DataTemplateColors);
        }

        [TestCase]
        public void ConvertMustacheLoopToRazor()
        {
            const string mustache =
@"{{#items}}
    <p>hello, world!</p>
{{/items}}";

            const string expectedRazor =
@"@foreach (var item in Model.Items) {
    <p>hello, world!</p>
}";
            AssertCorrectConversion(mustache, expectedRazor, DataTemplateColors);
        }

        [TestCase]
        public void ConvertMustacheVariableTagToRazor()
        {
            const string mustache = @"<h1>{{header}}</h1>";
            const string expectedRazor = @"<h1>@Model.Header</h1>";
            AssertCorrectConversion(mustache, expectedRazor, DataTemplateColors);
        }

        [TestCase]
        public void ConvertMustacheLineWithMultipleVariablesToRazor()
        {
            const string mustache = @"<p><span>{{header}}</span><span>{{header}}</span></p>";
            const string expectedRazor = @"<p><span>@Model.Header</span><span>@Model.Header</span></p>";
            AssertCorrectConversion(mustache, expectedRazor, DataTemplateColors);
        }

        [TestCase]
        public void ConvertMustacheAccessingObjectPropertyValueToRazor()
        {
            const string mustache = @"{{name.first}} {{name.last}}";
            const string expectedRazor = @"@Model.Name.First @Model.Name.Last";
            AssertCorrectConversion(mustache, expectedRazor, DataTemplatePerson);
        }

        private static void AssertCorrectConversion(string mustache, string expectedRazor, string dataTemplate)
        {
            var dataModel = (JObject)JsonConvert.DeserializeObject(dataTemplate);

            var convertedMustache = new MustacheDocument(mustache, dataModel).ToRazor();
            Assert.That(convertedMustache, Is.EqualTo(expectedRazor));
        }
    }
}