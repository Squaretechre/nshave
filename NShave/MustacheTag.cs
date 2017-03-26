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
        private readonly Scope _dataAccessScope;
        private readonly ScopeFormat _formattingScope;
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

        public MustacheTag(string mustacheTag, JObject dataModel, Scope dataAccessScope, ScopeFormat formattingScope)
        {
            _dataAccessScope = dataAccessScope;
            _formattingScope = formattingScope;
            _firstChar = mustacheTag.First();
            _key = mustacheTag.Substring(1, mustacheTag.Length - 1);

            if (_firstChar.Equals(EndSection))
            {
                _formattingScope.Leave(_key);
                _dataAccessScope.Leave(_key);
            }

            _type = dataAccessScope.IsDefault()
                ? dataModel[_key].Type
                : dataModel[dataAccessScope.Current()].First()[_key].Type;
        }

        public string ToRazor()
        {
            var razor = string.Empty;
            var razorPropertyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_key);

            if (_firstChar.Equals(EndSection))
            {
                return $"{_formattingScope.Indentation()}{RazorCloseBlock}{_formattingScope.NewLine()}";
            }

            var razorScopeMarker = _formattingScope.ScopeMarker();
            var indentation = _formattingScope.Indentation();
            var scopeName = _formattingScope.ScopeNameCorrectedForRendering();

            switch (_type)
            {
                case JTokenType.Boolean:
                    razor = _firstChar.Equals(InvertedSection)
                        ? string.Format(RazorFalseyIf, indentation, razorScopeMarker, scopeName, razorPropertyName, indentation)
                        : string.Format(RazorTruthyIf, indentation, razorScopeMarker, scopeName, razorPropertyName, indentation);
                    break;
                case JTokenType.Array:
                    var propertyNameSingular = _formattingScope.PluralToSingularName(razorPropertyName);
                    razor = string.Format(RazorForeach, indentation, "@", propertyNameSingular, scopeName, razorPropertyName, indentation);
                    _dataAccessScope.Enter(_key);
                    break;
            }

            _formattingScope.Enter(_key);
            return razor;
        }
    }
}