using System.IO;
using NShave.Tests.Support;
using Xunit;

namespace NShave.Tests.Tests
{
    public class PatternLabTests
    {
        [Fact]
        public void ShouldConvertPatternLabPageTemplateToRazor()
        {
            var workingDir = FilePath.WorkingDirectory();
            var mustache = File.ReadAllText($"{workingDir}homepage.mustache");
            var data = File.ReadAllText($"{workingDir}homepage.json");
            var expectedRazor = File.ReadAllText($"{workingDir}homepage.cshtml");
            ConversionAssertion.AssertCorrectWith(mustache, expectedRazor, data);
        }
    }
}