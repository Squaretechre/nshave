using System.Globalization;
using Xunit;

namespace NShave.Tests.Tests
{
    public class TextInfoExtensionsTests
    {
        [Fact]
        public void ShouldConvertFirstLetterOfSingleWordToUpperCase()
        {
            const string lowerCaseInput = "foo";
            const string expected = "Foo";
            var actualResult = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lowerCaseInput);
            Assert.Equal(expected, actualResult);
        }

        [Fact]
        public void ShouldConvertObjectPropertiesToUpperCaseWhenAccessed()
        {
            const string lowerCaseInput = "foo.bar";
            const string expected = "Foo.Bar";
            var actualResult = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lowerCaseInput);
            Assert.Equal(expected, actualResult);
        }

        [Fact]
        public void ShouldConvertPartialNamesToTitleCase()
        {
            const string lowerCaseInput = "_user";
            const string expected = "_User";
            var actualResult = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lowerCaseInput);
            Assert.Equal(expected, actualResult);
        }
    }
}