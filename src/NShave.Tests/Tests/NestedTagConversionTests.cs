using NShave.Tests.Support;
using Xunit;

namespace NShave.Tests.Tests
{
    public class NestedTagConversionTests
    {
        [Fact]
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

            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModel.ColorsStructured);
        }

        [Fact]
        public void ConvertMustacheWithNestedLoopOneLevelDeepToRazor()
        {
            
const string mustache = 
@"{{#posts}}
    <p>{{title}}</p>
    <ul>
        {{#categories}}
            <li>{{name}}</li>
        {{/categories}}
    </ul>
{{/posts}}";

        const string expectedRazor =
@"@foreach (var post in Model.Posts)
{
    <p>@post.Title</p>
    <ul>
    @foreach (var category in post.Categories)
    {
        <li>@category.Name</li>
    }

    </ul>
}";

            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModel.Posts);
        }

        [Fact]
        public void ConvertMustacheWithNestedLoopTwoLevelsDeepToRazor()
        {
const string mustache =
@"{{#posts}}
<p>{{title}}</p>
    <ul>
        {{#categories}}
        <li>{{name}}</li>
        {{/categories}}
    </ul>
    {{#authors}}
    <p>{{name}}</p>
        {{#socialmedia}}
        <span>{{url}}</span>
        {{/socialmedia}}
    {{/authors}}
{{/posts}}";

const string expectedRazor =
@"@foreach (var post in Model.Posts)
{
    <ul>
    @foreach (var category in post.Categories)
    {
        <li>@category.Name</li>
    }
    </ul>
    foreach (var author in post.Authors)
    {
        foreach (var socialmedia in author.SocialMedia)
        {
            <span>@socialmedia.Url</span>
        }
    }
}";
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModel.PostWithNestedLoops);
        }
    }
}