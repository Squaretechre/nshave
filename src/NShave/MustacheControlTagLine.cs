using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace NShave
{
    public class MustacheControlTagLine
    {
        private const char InvertedSectionToken = '^';
        private const char EndSectionToken = '/';
        private const string RazorCloseBlockToken = "}";

        private const string RazorTruthyIf =
            @"if ({0}.{1})
{2}{{";

        private const string RazorFalseyIf =
            @"if (!{0}.{1})
{2}{{";

        private const string RazorForEach =
            @"foreach (var {0} in {1}.{2})
{3}{{";

        private readonly Scope _dataAccessScope;
        private readonly JObject _dataModel;
        private readonly ScopePresentationFormat _formattingScopePresentation;

        private readonly string _templateLine;
        private char _firstCharOfTag;
        private string _tagKey;
        private JTokenType _type;

        public MustacheControlTagLine(string templateLine, JObject dataModel, Scope dataAccessScope,
            ScopePresentationFormat formattingScopePresentation)
        {
            _templateLine = templateLine;
            _dataModel = dataModel;
            _dataAccessScope = dataAccessScope;
            _formattingScopePresentation = formattingScopePresentation;
        }

        public string ToRazor()
        {
            _tagKey = Regex.Match(_templateLine, @"{{(.*)}}", RegexOptions.IgnoreCase).Groups[1].Value;

            _firstCharOfTag = _tagKey.First();
            _tagKey = _tagKey.Substring(1, _tagKey.Length - 1);

            var razor = string.Empty;
            var razorPropertyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_tagKey);

            if (EndSectionToken.Equals(_firstCharOfTag)) LeaveCurrentScope();
            if (EndSectionToken.Equals(_firstCharOfTag)) return RazorCloseBlock();

            _type = _dataAccessScope.IsDefault()
                ? _dataModel[_tagKey].Type
                : _dataModel.SelectToken(_dataAccessScope.AsJsonPath())[_tagKey].Type;

            var indentation = _formattingScopePresentation.Indentation();
            var scopeName = _formattingScopePresentation.ScopeNameCorrectedForRendering();

            razor = _formattingScopePresentation.ApplyScopeMarker(razor);

            switch (_type)
            {
                case JTokenType.Boolean:
                    razor = InvertedSectionToken.Equals(_firstCharOfTag)
                        ? $"{indentation}{razor}{string.Format(RazorFalseyIf, scopeName, razorPropertyName, indentation)}"
                        : $"{indentation}{razor}{string.Format(RazorTruthyIf, scopeName, razorPropertyName, indentation)}";
                    break;
                case JTokenType.Array:
                    var propertyNameSingular = _formattingScopePresentation.PluralToSingularName(razorPropertyName);
                    razor =
                        $"{indentation}{razor}{string.Format(RazorForEach, propertyNameSingular, scopeName, razorPropertyName, indentation)}";
                    _dataAccessScope.Enter(new ScopeType(_tagKey, TokenType.Array));
                    break;
            }

            _formattingScopePresentation.Enter(new ScopeType(_tagKey, TokenType.Formatting));
            return razor;
        }

        private void LeaveCurrentScope()
        {
            _formattingScopePresentation.Leave(_tagKey);
            _dataAccessScope.Leave(_tagKey);
        }

        private string RazorCloseBlock()
            => $"{_formattingScopePresentation.Indentation()}{RazorCloseBlockToken}";
    }
}