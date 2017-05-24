using NShave.Tests.Support;
using Xunit;

namespace NShave.Tests.Tests
{
    public class TagConversionTests
    {
        [Fact]
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

            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModels.ColorsStructured);
        }

        [Fact]
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

            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModels.ColorsStructured);
        }

        [Fact]
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
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModels.ColorsStructured);
        }

        [Fact]
        public void ConvertMustacheLoopWithDotSyntaxToRazor()
        {
            const string mustache =
@"{{#days}}
    <p>{{.}}</p>
{{/days}}";

            const string expectedRazor =
@"@foreach (var day in Model.Days)
{
    <p>@day</p>
}";
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModels.DaysOfTheWeek);
        }

        [Fact]
        public void ConvertMustacheVariableTagToRazor()
        {
            const string mustache = @"<h1>{{header}}</h1>";
            const string expectedRazor = @"<h1>@Model.Header</h1>";
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModels.ColorsStructured);
        }

        [Fact]
        public void ConvertMustacheLineWithMultipleVariablesToRazor()
        {
            const string mustache = @"<p><span>{{header}}</span><span>{{header}}</span></p>";
            const string expectedRazor = @"<p><span>@Model.Header</span><span>@Model.Header</span></p>";
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModels.ColorsStructured);
        }

        [Fact]
        public void ConvertMustacheAccessingObjectPropertyValueToRazor()
        {
            const string mustache = @"{{name.first}} {{name.last}}";
            const string expectedRazor = @"@Model.Name.First @Model.Name.Last";
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModels.Person);
        }

        [Fact]
        public void ConvertMustacheCommentsToRazor()
        {
            const string mustache = @"<h1>Today{{! ignore me }}.</h1>";
            const string expectedRazor = @"<h1>Today@* ignore me *@.</h1>";
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModels.Person);
        }

        [Fact]
        public void ConvertMustachePartialToRazor()
        {
            const string mustache = @"{{> user}}";
            const string expectedRazor = @"@Html.Partial(""_User"", (object)Model)";
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModels.Person);
        }

		[Fact]
		public void ConvertMustacheInlineIfToRazor()
		{
			const string mustache = @"<select id=""{{ select.name }}"" name=""{{ select.name }}"" {{# select.class }} class=""{{ select.class }}"" {{/ select.class }}>";
			const string expectedRazor = @"<select id=""@Model.Name"" name=""@Model.Name"" @(!string.IsNullOrEmpty(Model.Class) ? string.Format(""class={0}"", Model.Class) : string.Empty)></select>";
			ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModels.Person);
		}
	}
}