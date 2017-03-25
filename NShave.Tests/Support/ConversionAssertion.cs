using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NShave.Tests.Support
{
    internal static class ConversionAssertion
    {
        public static void AssertCorrectWith(string mustache, string expectedRazor, string dataModel)
        {
            var scope = new Scope();
            var model = (JObject) JsonConvert.DeserializeObject(dataModel);
            var convertedMustache = new MustacheDocument(mustache, model, scope).ToRazor();
            Assert.That(convertedMustache, Is.EqualTo(expectedRazor));
        }
    }
}