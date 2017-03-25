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
            var model = (JObject)JsonConvert.DeserializeObject(DataModels.Colors);
            var convertedMustache = new MustacheDocument(mustache, model, scope).ToRazor();

            const string expectedScope = "items";
            var actualScope = scope.Current();
            Assert.That(actualScope, Is.EqualTo(expectedScope));
        }

        [TestCase]
        public void ScopeShouldReturnToDefaultAfterEnteringAndLeavingANewScopeFromDefault()
        {
            const string mustache =
@"{{#items}}
    <p>hello, world!</p>
{{/items}}";

            var scope = new Scope();
            var model = (JObject)JsonConvert.DeserializeObject(DataModels.Colors);
            var convertedMustache = new MustacheDocument(mustache, model, scope).ToRazor();

            var actualScope = scope.Current();
            Assert.That(actualScope, Is.EqualTo(DefaultScopeName));
        }
    }
}