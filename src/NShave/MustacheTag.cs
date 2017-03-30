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
@"if ({0}.{1})
{2}{{";

        private const string RazorFalseyIf =
@"if (!{0}.{1})
{2}{{";

        private const string RazorForEach =
@"foreach (var {0} in {1}.{2})
{3}{{";

        public MustacheTag(string mustacheTag, JObject dataModel, Scope dataAccessScope, ScopeFormat formattingScope)
        {
            _dataAccessScope = dataAccessScope;
            _formattingScope = formattingScope;
            _firstCharOfTag = mustacheTag.First();
            _tagKey = mustacheTag.Substring(1, mustacheTag.Length - 1);

            if (_firstCharOfTag.Equals(EndSectionToken)) LeaveCurrentScope();

            _type = dataAccessScope.IsDefault()
                ? dataModel[_tagKey].Type
                : dataModel.SelectToken(dataAccessScope.AsJsonPath())[_tagKey].Type;
        }

        public string ToRazor()
        {
            var razor = string.Empty;
            var razorPropertyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_tagKey);

            if (_firstCharOfTag.Equals(EndSectionToken)) return RazorCloseBlock();

            var indentation = _formattingScope.Indentation();
            var scopeName = _formattingScope.ScopeNameCorrectedForRendering();

            razor = _formattingScope.ApplyScopeMarker(razor);

            switch (_type)
            {
                case JTokenType.Boolean:
                    razor = _firstCharOfTag.Equals(InvertedSectionToken)
                        ? $"{indentation}{razor}{string.Format(RazorFalseyIf, scopeName, razorPropertyName, indentation)}"
                        : $"{indentation}{razor}{string.Format(RazorTruthyIf, scopeName, razorPropertyName, indentation)}";
                    break;
                case JTokenType.Array:
                    var propertyNameSingular = _formattingScope.PluralToSingularName(razorPropertyName);
                    razor = $"{indentation}{razor}{string.Format(RazorForEach, propertyNameSingular, scopeName, razorPropertyName, indentation)}";
                    _dataAccessScope.Enter(new ScopeType(_tagKey, TokenType.Array));
                    break;
            }

            _formattingScope.Enter(new ScopeType(_tagKey, TokenType.Formatting));
            return razor;
        }

        private void LeaveCurrentScope()
        {
            _formattingScope.Leave(_tagKey);
            _dataAccessScope.Leave(_tagKey);
        }

        private string RazorCloseBlock() 
            => $"{_formattingScope.Indentation()}{RazorCloseBlockToken}";
    }
}