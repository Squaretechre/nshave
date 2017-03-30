using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NShave.Tests.Support;
using Xunit;

namespace NShave.Tests.Tests
{
    public class ScopeTests
    {
        private readonly ScopeType _defaultScopeType = new ScopeType("Model", TokenType.Default);
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
            var actualScope = (new Scope()).Current();
            Assert.Equal(actualScope.Name, _defaultScopeType.Name);
        }

        [Fact]
        public void ScopeShouldChangeWhenEnteringAnArrayAndRemainInThatScopeIfTagIsntClosed() 
            => AssertInCorrectScopeName(MustacheLoopOnLevelDeepNotClosed, ArrayScopeTypeWithName("items"));

        [Fact]
        public void ScopeShouldReturnToDefaultAfterEnteringAndLeavingANewScopeFromDefaultScope() 
            => AssertInCorrectScopeName(MustacheLoopOneLevelDeepClosed, _defaultScopeType);

        [Fact]
        public void ShouldReportNestingLevelOfOneWhenEnteringANewScopeFromDefaultScope() 
            => AssertCorrectNestingDepth(MustacheLoopOnLevelDeepNotClosed, 1);

        [Fact]
        public void ShouldReportNestingLevelOfZeroWhenInDefaultScope() 
            => AssertCorrectNestingDepth(TemplateOneLineWithOneVariable, 0);

        [Fact]
        public void ShouldGenerateJsonPathForCurrentScopeStackWithTwoScopesAdded()
        {
            var scope = new Scope();
            scope.Enter(ArrayScopeTypeWithName("posts"));
            scope.Enter(ArrayScopeTypeWithName("authors"));

            const string expectedPath = "posts[0].authors[0]";
            var actualPath = scope.AsJsonPath();
            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public void ShouldGenerateJsonPathForCurrentScopeStackWithFiveScopesAdded()
        {

            var scope = new Scope();
            scope.Enter(ArrayScopeTypeWithName("foo"));
            scope.Enter(ArrayScopeTypeWithName("bar"));
            scope.Enter(ArrayScopeTypeWithName("baz"));
            scope.Enter(ArrayScopeTypeWithName("qux"));
            scope.Enter(ArrayScopeTypeWithName("quux"));

            const string expectedPath = "foo[0].bar[0].baz[0].qux[0].quux[0]";
            var actualPath = scope.AsJsonPath();
            Assert.Equal(expectedPath, actualPath);
        }

        private static void AssertInCorrectScopeName(string mustache, ScopeType expectedScope)
        {
            var actualScope = ResultingScopeForTemplate(mustache).Current();
            Assert.Equal(actualScope.Name, expectedScope.Name);
        }

        private static void AssertCorrectNestingDepth(string mustache, int expectedDepth)
        {
            var actualScope = ResultingScopeForTemplate(mustache);
            Assert.Equal(actualScope.Nesting(), expectedDepth);
        }

        private static ScopeType ArrayScopeTypeWithName(string name) => new ScopeType(name, TokenType.Array);
 
        private static Scope ResultingScopeForTemplate(string mustache)
        {
            var dataAccessScope = new Scope();
            var formattingScope = new Scope();
            var formatting = new ScopePresentationFormat(dataAccessScope, formattingScope);
            var model = (JObject)JsonConvert.DeserializeObject(DataModel.ColorsStructured);
            new MustacheDocument(mustache, model, dataAccessScope, formatting).ToRazor();
            return dataAccessScope;
        }
    }
}