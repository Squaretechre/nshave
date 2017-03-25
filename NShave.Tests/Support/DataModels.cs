namespace NShave.Tests.Support
{
    internal static class DataModels
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
    }
}