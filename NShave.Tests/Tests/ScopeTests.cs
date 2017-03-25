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
            var scope = new Scope();
            var formatting = new ScopeFormat(scope);
            var model = (JObject)JsonConvert.DeserializeObject(DataModels.ColorsStructured);
            var convertedMustache = new MustacheDocument(mustache, model, scope, formatting).ToRazor();

            const string expectedScope = "items";
            var actualScope = scope.Current();
            Assert.That(actualScope, Is.EqualTo(expectedScope));
        }

        [TestCase]
        public void ScopeShouldReturnToDefaultAfterEnteringAndLeavingANewScopeFromDefaultScope()
        {
            const string mustache =
@"{{#items}}
    <p>hello, world!</p>
{{/items}}";

            var scope = new Scope();
            var formatting = new ScopeFormat(scope);
            var model = (JObject)JsonConvert.DeserializeObject(DataModels.ColorsStructured);
            var convertedMustache = new MustacheDocument(mustache, model, scope, formatting).ToRazor();

            var actualScope = scope.Current();
            Assert.That(actualScope, Is.EqualTo(DefaultScopeName));
        }

        [TestCase]
        public void ShouldReportNestingLevelOfOneWhenEnteringANewScopeFromDefaultScope()
        {
            const string mustache =
@"{{#items}}
    <p>hello, world!</p>
";
            var scope = new Scope();
            var formatting = new ScopeFormat(scope);
            var model = (JObject)JsonConvert.DeserializeObject(DataModels.ColorsStructured);
            var convertedMustache = new MustacheDocument(mustache, model, scope, formatting).ToRazor();
            Assert.That(scope.Nesting(), Is.EqualTo(1));
        }

        [TestCase]
        public void ShouldReportNestingLevelOfZeroWhenInDefaultScope()
        {
            const string mustache = "<p>{{heading}}</p>";
            var scope = new Scope();
            var formatting = new ScopeFormat(scope);
            var model = (JObject)JsonConvert.DeserializeObject(DataModels.ColorsStructured);
            var convertedMustache = new MustacheDocument(mustache, model, scope, formatting).ToRazor();
            Assert.That(scope.Nesting(), Is.EqualTo(0));
        }
    }
}