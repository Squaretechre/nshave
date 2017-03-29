using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace NShave
{
    public class MustacheTag
    {
        private const char InvertedSectionToken = '^';
        private const char EndSectionToken = '/';
        private const string RazorCloseBlockToken = "}";
        private readonly char _firstCharOfTag;
        private readonly string _tagKey;
        private readonly Scope _dataAccessScope;
        private readonly ScopeFormat _formattingScope;
        private readonly JTokenType _type;

        private const string RazorTruthyIf = 
@"{0}{1}if ({2}.{3})
{4}{{";

        private const string RazorFalseyIf =
@"{0}{1}if (!{2}.{3})
{4}{{";

        private const string RazorForEach =
@"{0}{1}foreach (var {2} in {3}.{4})
{5}{{";

        public MustacheTag(string mustacheTag, JObject dataModel, Scope dataAccessScope, ScopeFormat formattingScope)
        {
            _dataAccessScope = dataAccessScope;
            _formattingScope = formattingScope;
            _firstCharOfTag = mustacheTag.First();
            _tagKey = mustacheTag.Substring(1, mustacheTag.Length - 1);

            if (_firstCharOfTag.Equals(EndSectionToken)) LeaveCurrentScopes();

            _type = dataAccessScope.IsDefault()
                ? dataModel[_tagKey].Type
                : dataModel.SelectToken(dataAccessScope.AsJsonPath())[_tagKey].Type;
        }

        public string ToRazor()
        {
            var razor = string.Empty;
            var razorPropertyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_tagKey);

            if (_firstCharOfTag.Equals(EndSectionToken)) return RazorCloseBlock();

            var razorScopeMarker = _formattingScope.ScopeMarker();
            var indentation = _formattingScope.Indentation();
            var scopeName = _formattingScope.ScopeNameCorrectedForRendering();

            switch (_type)
            {
                case JTokenType.Boolean:
                    razor = _firstCharOfTag.Equals(InvertedSectionToken)
                        ? string.Format(RazorFalseyIf, indentation, razorScopeMarker, scopeName, razorPropertyName, indentation)
                        : string.Format(RazorTruthyIf, indentation, razorScopeMarker, scopeName, razorPropertyName, indentation);
                    break;
                case JTokenType.Array:
                    var propertyNameSingular = _formattingScope.PluralToSingularName(razorPropertyName);
                    razor = string.Format(RazorForEach, indentation, "@", propertyNameSingular, scopeName, razorPropertyName, indentation);
                    _dataAccessScope.Enter(_tagKey);
                    break;
            }

            _formattingScope.Enter(_tagKey);
            return razor;
        }

        private void LeaveCurrentScopes()
        {
            _formattingScope.Leave(_tagKey);
            _dataAccessScope.Leave(_tagKey);
        }

        private string RazorCloseBlock() 
            => $"{_formattingScope.Indentation()}{RazorCloseBlockToken}{_formattingScope.NewLine()}";
    }
}