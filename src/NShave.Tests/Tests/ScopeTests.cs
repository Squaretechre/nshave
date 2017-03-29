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

        [Fact]
        public void ShouldGenerateJsonPathForCurrentScopeStackWithTwoScopesAdded()
        {
            const string scopePosts = "posts";
            const string scopeAuthors = "authors";
            var scope = new Scope();
            scope.Enter(scopePosts);
            scope.Enter(scopeAuthors);

            const string expectedPath = "posts[0].authors[0]";
            var actualPath = scope.AsJsonPath();
            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public void ShouldGenerateJsonPathForCurrentScopeStackWithFiveScopesAdded()
        {
            const string scope1 = "foo";
            const string scope2 = "bar";
            const string scope3 = "baz";
            const string scope4 = "qux";
            const string scope5 = "quux";
            var scope = new Scope();
            scope.Enter(scope1);
            scope.Enter(scope2);
            scope.Enter(scope3);
            scope.Enter(scope4);
            scope.Enter(scope5);

            const string expectedPath = "foo[0].bar[0].baz[0].qux[0].quux[0]";
            var actualPath = scope.AsJsonPath();
            Assert.Equal(expectedPath, actualPath);
        }

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