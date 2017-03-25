using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace NShave
{
    public class MustacheTag
    {
        private const char Section = '#';
        private const char InvertedSection = '^';
        private const char EndSection = '/';
        private const string RazorCloseBlock = "}";
        private readonly char _firstChar;
        private readonly string _key;
        private readonly JTokenType _type;

        public MustacheTag(string mustacheTag, JObject dataModel)
        {
            _firstChar = mustacheTag.First();
            _key = mustacheTag.Substring(1, mustacheTag.Length - 1);
            _type = dataModel[_key].Type;
        }

        public string ToRazor()
        {
            var razor = string.Empty;
            var razorPropertyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_key);

            if (_firstChar.Equals(EndSection)) return RazorCloseBlock;

            switch (_type)
            {
                case JTokenType.Boolean:
                    razor = _firstChar.Equals(InvertedSection)
                        ? $"@if (!Model.{razorPropertyName}) {{"
                        : $"@if (Model.{razorPropertyName}) {{";
                    break;
                case JTokenType.Array:
                    var singularName = PluralToSingularName(razorPropertyName);
                    razor = $"@foreach (var {singularName} in Model.{razorPropertyName}) {{";
                    break;
            }
            return razor;
        }

        private static string PluralToSingularName(string razorPropertyName) 
            => razorPropertyName
            .Substring(0, razorPropertyName.Length - 1)
            .Insert(0, razorPropertyName.ToLower().First().ToString())
            .Remove(1, 1);
    }
}