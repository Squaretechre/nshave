using NShave.Tests.Support;
using NUnit.Framework;

namespace NShave.Tests.Tests
{
    [TestFixture]
    public class CoreConversionTests
    {
        [TestCase]
        public void ConvertMustacheTruthyIfStatementToRazor()
        {
            const string mustache =
@"{{#empty}}
    <p>hello, world!</p>
{{/empty}}";

            const string expectedRazor =
@"@if (Model.Empty)
{
    <p>hello, world!</p>
}";

            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModel.ColorsStructured);
        }

        [TestCase]
        public void ConvertMustacheFalseyIfStatementToRazor()
        {
            const string mustache =
@"{{^empty}}
    <p>hello, world!</p>
{{/empty}}";

            const string expectedRazor =
@"@if (!Model.Empty)
{
    <p>hello, world!</p>
}";

            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModel.ColorsStructured);
        }

        [TestCase]
        public void ConvertMustacheLoopToRazor()
        {
            const string mustache =
@"{{#items}}
    <p>hello, world!</p>
{{/items}}";

            const string expectedRazor =
@"@foreach (var item in Model.Items)
{
    <p>hello, world!</p>
}";
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModel.ColorsStructured);
        }

        [TestCase]
        public void ConvertMustacheVariableTagToRazor()
        {
            const string mustache = @"<h1>{{header}}</h1>";
            const string expectedRazor = @"<h1>@Model.Header</h1>";
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModel.ColorsStructured);
        }

        [TestCase]
        public void ConvertMustacheLineWithMultipleVariablesToRazor()
        {
            const string mustache = @"<p><span>{{header}}</span><span>{{header}}</span></p>";
            const string expectedRazor = @"<p><span>@Model.Header</span><span>@Model.Header</span></p>";
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModel.ColorsStructured);
        }

        [TestCase]
        public void ConvertMustacheAccessingObjectPropertyValueToRazor()
        {
            const string mustache = @"{{name.first}} {{name.last}}";
            const string expectedRazor = @"@Model.Name.First @Model.Name.Last";
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModel.Person);
        }

        [TestCase]
        public void ConvertMustacheCommentsToRazor()
        {
            const string mustache = @"<h1>Today{{! ignore me }}.</h1>";
            const string expectedRazor = @"<h1>Today@* ignore me *@.</h1>";
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModel.Person);
        }

        [TestCase]
        public void ConvertMustachePartialToRazor()
        {
            const string mustache = @"{{> user}}";
            const string expectedRazor = @"@Html.Partial(""_User"", Model)";
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModel.Person);
        }
    }
}