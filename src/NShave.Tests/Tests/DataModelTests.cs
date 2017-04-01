using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NShave.Mustache;
using NShave.Scope;
using NShave.Tests.Support;
using Xunit;

namespace NShave.Tests.Tests
{
    public class DataModelTests
    {
        [Fact]
        public void ShouldResolveTypeBooleanIfTokenTypeIsObject()
        {
            var scope = new ScopeDataModel();
            var data = File.ReadAllText($"{FilePath.WorkingDirectory()}homepage.json");
            var jsonData = (JObject) JsonConvert.DeserializeObject(data);
            var dataModel = new DataModel(scope, jsonData);

            const JTokenType expectedType = JTokenType.Boolean;
            var actualType = dataModel.TypeForTagKey("latestPosts");
            Assert.Equal(expectedType, actualType);
        }
    }
}