using NShave.Tests.Support;
using Xunit;

namespace NShave.Console.Tests
{
    public class ConsoleTests : IDomainAdapter
    {
        public string Data { get; set; }
        public string MustacheTemplate { get; set; }

        [Fact]
        public void ShouldConvertASingleMustacheTemplateToRazor()
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
    <p>@post.Title</p>
    <ul>
    @foreach (var category in post.Categories)
    {
        <li>@category.Name</li>
    }
    </ul>
    foreach (var author in post.Authors)
    {
        <p>@author.Name</p>
        foreach (var socialmedia in author.Socialmedia)
        {
            <span>@socialmedia.Url</span>
        }
    }
}";
            var adapter = this;
            adapter.MustacheTemplate = mustache;
            adapter.Data = DataModel.PostWithNestedLoops;
            var domain = new Domain(adapter);
            var actualConvertedRazor = string.Empty;
            domain.ToRazor(convertedMustache => actualConvertedRazor = convertedMustache);
            Assert.Equal(expectedRazor, actualConvertedRazor);
        }
    }
}