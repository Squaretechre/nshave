using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NShave.Mustache;
using NShave.Scope;
using Xunit;

namespace NShave.Tests.Support
{
    internal static class ConversionAssertion
    {
        public static void AssertCorrectWith(string mustache, string expectedRazor, string dataModel)
        {
            var dataAccessScope = new ScopeDataModel();
            var formattingScope = new ScopeDataModel();
            var formatting = new ScopePresentation(dataAccessScope, formattingScope);
            var model = (JObject)JsonConvert.DeserializeObject(dataModel);
            var convertedMustache = new MustacheDocument(mustache, model, dataAccessScope, formatting).ToRazor();
            Assert.Equal(expectedRazor, convertedMustache);
        }
    }
}