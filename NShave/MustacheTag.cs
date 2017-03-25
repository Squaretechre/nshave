using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace NShave
{
    public class MustacheTag
    {
        private readonly string _name;
        private readonly JTokenType _type;

        public MustacheTag(string name, JTokenType type)
        {
            _name = name;
            _type = type;
        }

        public string ToRazor()
        {
            var razor = string.Empty;
            var razorPropertyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_name);
            switch (_type)
            {
                case JTokenType.Boolean:
                    razor = $"@if (Model.{razorPropertyName}) {{";
                    break;
                case JTokenType.Array:
                    var singularName = PluralToSingularName(razorPropertyName);
                    razor = $"@foreach (var {singularName} in Model.{razorPropertyName}) {{";
                    break;
            }
            return razor;
        }

        private static string PluralToSingularName(string razorPropertyName)
        {
            return razorPropertyName
                .Substring(0, razorPropertyName.Length - 1)
                .Insert(0, razorPropertyName.ToLower().First().ToString())
                .Remove(1, 1);
        }
    }
}