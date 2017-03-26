using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NShave.Tests.Support;
using NUnit.Framework;

namespace NShave.Tests.Tests
{
    [TestFixture]
    internal class ScopeTests
    {
        private const string DefaultScopeName = "Model";

        [TestCase]
        public void DefaultScopeNameShouldBeModel()
        {
            var actualScopeName = (new Scope()).Current();
            Assert.That(actualScopeName, Is.EqualTo(DefaultScopeName));
        }

        [TestCase]
        public void ScopeShouldChangeWhenEnteringAnArrayAndRemainInThatScopeIfTagIsntClosed()
        {
            const string mustache =
@"{{#items}}
    <p>hello, world!</p>
";
            var dataAccessScope = new Scope();
            var formattingScope = new Scope();
            var formatting = new ScopeFormat(dataAccessScope, formattingScope);
            var model = (JObject)JsonConvert.DeserializeObject(DataModel.ColorsStructured);
            var convertedMustache = new MustacheDocument(mustache, model, dataAccessScope, formatting).ToRazor();

            const string expectedScope = "items";
            var actualScope = dataAccessScope.Current();
            Assert.That(actualScope, Is.EqualTo(expectedScope));
        }

        [TestCase]
        public void ScopeShouldReturnToDefaultAfterEnteringAndLeavingANewScopeFromDefaultScope()
        {
            const string mustache =
@"{{#items}}
    <p>hello, world!</p>
{{/items}}";

            var dataAccessScope = new Scope();
            var formattingScope = new Scope();
            var formatting = new ScopeFormat(dataAccessScope, formattingScope);
            var model = (JObject)JsonConvert.DeserializeObject(DataModel.ColorsStructured);
            var convertedMustache = new MustacheDocument(mustache, model, dataAccessScope, formatting).ToRazor();

            var actualScope = dataAccessScope.Current();
            Assert.That(actualScope, Is.EqualTo(DefaultScopeName));
        }

        [TestCase]
        public void ShouldReportNestingLevelOfOneWhenEnteringANewScopeFromDefaultScope()
        {
            const string mustache =
@"{{#items}}
    <p>hello, world!</p>
";
            var dataAccessScope = new Scope();
            var formattingScope = new Scope();
            var formatting = new ScopeFormat(dataAccessScope, formattingScope);
            var model = (JObject)JsonConvert.DeserializeObject(DataModel.ColorsStructured);
            var convertedMustache = new MustacheDocument(mustache, model, dataAccessScope, formatting).ToRazor();
            Assert.That(dataAccessScope.Nesting(), Is.EqualTo(1));
        }

        [TestCase]
        public void ShouldReportNestingLevelOfZeroWhenInDefaultScope()
        {
            const string mustache = "<p>{{heading}}</p>";
            var dataAccessScope = new Scope();
            var formattingScope = new Scope();
            var formatting = new ScopeFormat(dataAccessScope, formattingScope);
            var model = (JObject)JsonConvert.DeserializeObject(DataModel.ColorsStructured);
            var convertedMustache = new MustacheDocument(mustache, model, dataAccessScope, formatting).ToRazor();
            Assert.That(dataAccessScope.Nesting(), Is.EqualTo(0));
        }
    }
}