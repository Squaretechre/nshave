namespace NShave.Tests.Support
{
    internal static class DataModel
    {
        public const string ColorsStructured =
@"{
  ""header"": ""ColorsStructured"",
  ""items"": [
      {""name"": ""red"", ""link"": true, ""first"": true, ""url"": ""#Red""},
      {""name"": ""green"", ""link"": true, ""url"": ""#Green""},
      {""name"": ""blue"", ""link"": true, ""url"": ""#Blue""}
  ],
  ""empty"": false
}";

        public const string ColorsUnstructured =
@"{
  ""header"": ""ColorsStructured"",
  ""items"": [
      {""name"": ""red"", ""first"": true, ""url"": ""#Red""},
      {""name"": ""green"", ""link"": true, ""url"": ""#Green""},
      {""name"": ""blue"", ""link"": true, ""url"": ""#Blue""}
  ],
  ""empty"": false
}";

        public const string Person = 
@"{
  ""name"": {
    ""first"": ""John"",
    ""last"": ""Doe""
  }
}";

        public const string Posts =
@"{
  ""posts"": [
      {""title"": ""foo"", ""categories"": [{ ""name"": ""tech"" },
    { ""name"": ""mobile"" }]},
      {""title"": ""bar"", ""categories"": [{ ""name"": ""infosec"" },
    { ""name"": ""mobile"" }, { ""name"": ""web"" }]}
  ]
}";
    }
}