using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NShave.Tests.Support;
using Xunit;

namespace NShave.Tests.Tests
{
    public class ScopeTests
    {
        private const string DefaultScopeName = "Model";
        private const string TemplateOneLineWithOneVariable = "<p>{{heading}}</p>";
        private const string MustacheLoopOneLevelDeepClosed = 
@"{{#items}}
    <p>hello, world!</p>
{{/items}}";

        private const string MustacheLoopOnLevelDeepNotClosed = 
@"{{#items}}
    <p>hello, world!</p>
";

        [Fact]
        public void DefaultScopeNameShouldBeModel()
        {
            var actualScopeName = (new Scope()).Current();
            Assert.Equal(actualScopeName, DefaultScopeName);
        }

        [Fact]
        public void ScopeShouldChangeWhenEnteringAnArrayAndRemainInThatScopeIfTagIsntClosed() 
            => AssertInCorrectScopeName(MustacheLoopOnLevelDeepNotClosed, "items");

        [Fact]
        public void ScopeShouldReturnToDefaultAfterEnteringAndLeavingANewScopeFromDefaultScope() 
            => AssertInCorrectScopeName(MustacheLoopOneLevelDeepClosed, DefaultScopeName);

        [Fact]
        public void ShouldReportNestingLevelOfOneWhenEnteringANewScopeFromDefaultScope() 
            => AssertCorrectNestingDepth(MustacheLoopOnLevelDeepNotClosed, 1);

        [Fact]
        public void ShouldReportNestingLevelOfZeroWhenInDefaultScope() 
            => AssertCorrectNestingDepth(TemplateOneLineWithOneVariable, 0);

        private static void AssertInCorrectScopeName(string mustache, string expectedScopeName)
        {
            var actualScope = ResultingScopeForTemplate(mustache).Current();
            Assert.Equal(actualScope, expectedScopeName);
        }

        private static void AssertCorrectNestingDepth(string mustache, int expectedDepth)
        {
            var actualScope = ResultingScopeForTemplate(mustache);
            Assert.Equal(actualScope.Nesting(), expectedDepth);
        }

        private static Scope ResultingScopeForTemplate(string mustache)
        {
            var dataAccessScope = new Scope();
            var formattingScope = new Scope();
            var formatting = new ScopeFormat(dataAccessScope, formattingScope);
            var model = (JObject)JsonConvert.DeserializeObject(DataModel.ColorsStructured);
            new MustacheDocument(mustache, model, dataAccessScope, formatting).ToRazor();
            return dataAccessScope;
        }
    }
}