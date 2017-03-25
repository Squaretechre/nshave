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
        private readonly Scope _scope;
        private readonly ScopeFormat _formatting;
        private readonly JTokenType _type;

        private const string RazorTruthyIf = 
@"{0}{1}if ({2}.{3})
{4}{{";

        private const string RazorFalseyIf =
@"{0}{1}if (!{2}.{3})
{4}{{";

        private const string RazorForeach =
@"{0}{1}foreach (var {2} in {3}.{4})
{5}{{";

        public MustacheTag(string mustacheTag, JObject dataModel, Scope scope, ScopeFormat formatting)
        {
            _scope = scope;
            _formatting = formatting;
            _firstChar = mustacheTag.First();
            _key = mustacheTag.Substring(1, mustacheTag.Length - 1);
            if(_firstChar.Equals(EndSection)) _scope.Leave(_key);
            _type = scope.IsDefault()
                ? dataModel[_key].Type
                : dataModel[scope.Current()].First()[_key].Type;
        }

        public string ToRazor()
        {
            var razor = string.Empty;
            var razorPropertyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_key);

            if (_firstChar.Equals(EndSection))
            {
                return $"{_formatting.Indentation()}{RazorCloseBlock}{_formatting.NewLine()}";
            }

            var razorScopeMarker = _formatting.ScopeMarker();
            var indentation = _formatting.Indentation();

            switch (_type)
            {
                case JTokenType.Boolean:
                    var scopeName = _formatting.ScopeNameCorrectedForRendering();
                    razor = _firstChar.Equals(InvertedSection)
                        ? string.Format(RazorFalseyIf, indentation, razorScopeMarker, scopeName, razorPropertyName, indentation)
                        : string.Format(RazorTruthyIf, indentation, razorScopeMarker, scopeName, razorPropertyName, indentation);
                    break;
                case JTokenType.Array:
                    var propertyNameSingular = _formatting.PluralToSingularName(razorPropertyName);
                    razor = string.Format(RazorForeach, indentation, razorScopeMarker, propertyNameSingular, _scope.Current(), razorPropertyName, indentation);
                    _scope.Enter(_key);
                    break;
            }
            return razor;
        }
    }
}