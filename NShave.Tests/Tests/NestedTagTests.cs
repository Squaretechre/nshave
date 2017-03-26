using NShave.Tests.Support;
using NUnit.Framework;

namespace NShave.Tests.Tests
{
    [TestFixture]
    internal class NestedTagTests
    {
        [TestCase]
        public void ConvertMustacheLoopWithVariablesToRazor()
        {
            const string mustache =
@"{{#items}}
    {{#first}}
        <li><strong>{{name}}</strong></li>
    {{/first}}
    {{#link}}
        <li><a href=""{{url}}"">{{name}}</a></li>
    {{/link}}
{{/items}}";

            const string expectedRazor =
@"@foreach (var item in Model.Items)
{
    if (item.First)
    {
        <li><strong>@item.Name</strong></li>
    }

    if (item.Link)
    {
        <li><a href=""@item.Url"">@item.Name</a></li>
    }

}";
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModels.ColorsStructured);
        }
    }
}