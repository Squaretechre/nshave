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

            var convertedMustache = new MustacheDocument(mustache, dataModel).ToRazor();
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

            var dataModel = (JObject) JsonConvert.DeserializeObject(DataTemplate);

            var convertedMustache = new MustacheDocument(mustache, dataModel).ToRazor();
            Assert.That(convertedMustache, Is.EqualTo(razor));
        }

        [TestCase]
        public void ConvertMustacheModelValueWithRazorModelValue()
        {
            const string mustache = @"<h1>{{header}}</h1>";
            const string razor = @"<h1>@Model.header</h1>";

            var dataModel = (JObject) JsonConvert.DeserializeObject(DataTemplate);

            var convertedMustache = new MustacheDocument(mustache, dataModel).ToRazor();
            Assert.That(convertedMustache, Is.EqualTo(razor));
        }

        [TestCase]
        public void ConvertMustacheLineWithMultipleModelValuesToRazor()
        {
            const string mustache = @"<p><span>{{header}}</span><span>{{header}}</span></p>";
            const string expectedRazor = @"<p><span>@Model.header</span><span>@Model.header</span></p>";

            var dataModel = (JObject) JsonConvert.DeserializeObject(DataTemplate);

            var convertedMustache = new MustacheDocument(mustache, dataModel).ToRazor();
            Assert.That(convertedMustache, Is.EqualTo(expectedRazor));
        }
    }
}