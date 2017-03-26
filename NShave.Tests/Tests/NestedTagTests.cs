﻿using NShave.Tests.Support;
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
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, DataModel.ColorsStructured);
        }

        [TestCase]
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
    }
}