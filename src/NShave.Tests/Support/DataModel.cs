namespace NShave.Tests.Support
{
    public static class DataModel
    {
        public static string ColorsStructured =
@"{
  ""header"": ""ColorsStructured"",
  ""items"": [
      {""name"": ""red"", ""link"": true, ""first"": true, ""url"": ""#Red""},
      {""name"": ""green"", ""link"": true, ""url"": ""#Green""},
      {""name"": ""blue"", ""link"": true, ""url"": ""#Blue""}
  ],
  ""empty"": false
}";

        public static string ColorsUnstructured =
@"{
  ""header"": ""ColorsStructured"",
  ""items"": [
      {""name"": ""red"", ""first"": true, ""url"": ""#Red""},
      {""name"": ""green"", ""link"": true, ""url"": ""#Green""},
      {""name"": ""blue"", ""link"": true, ""url"": ""#Blue""}
  ],
  ""empty"": false
}";

        public static string Person =
@"{
  ""name"": {
    ""first"": ""John"",
    ""last"": ""Doe""
  }
}";

        public static string Posts =
@"{
  ""posts"": [
      {""title"": ""foo"", ""categories"": [{ ""name"": ""tech"" },
    { ""name"": ""mobile"" }]},
      {""title"": ""bar"", ""categories"": [{ ""name"": ""infosec"" },
    { ""name"": ""mobile"" }, { ""name"": ""web"" }]}
  ]
}";

        public static string PostWithNestedLoops =
@"{
    ""posts"": [
        {
            ""categories"": [""foo"", ""bar"", ""baz""],
            ""authors"": [
                 {
                     ""name"": ""foo"",
                     ""socialmedia"": [
                         ""http://www.twitter.com/"",
                         ""http://www.facebook.com/""
                     ]
                 },
                 {
                     ""name"": ""bar"",
                     ""socialmedia"": [
                         ""http://www.weibo.com""
                     ]
                 }
            ]
        }
    ]
}";

        public static string DaysOfTheWeek =
@"{
  ""days"": [""mon"", ""tue"",""wed"",""thur"", ""fri"",""sat"",""sun""]
}";
    }
}