using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using NShave.Scope;
using NShave.Extensions;

namespace NShave.Mustache
{
    public class MustacheBehaviourTagLine
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

        private readonly ScopeDataModel _dataAccessScope;
        private readonly JObject _dataModel;
        private readonly ScopePresentation _formattingScopePresentation;

        private readonly string _templateLine;
        private char _firstCharOfTag;
        private string _leadingWhiteSpace;
        private string _tagKey;
        private JTokenType _type;

        public MustacheBehaviourTagLine(string templateLine, JObject dataModel, ScopeDataModel dataAccessScope,
            ScopePresentation formattingScopePresentation)
        {
            _templateLine = templateLine;
            _dataModel = dataModel;
            _dataAccessScope = dataAccessScope;
            _formattingScopePresentation = formattingScopePresentation;
        }

        public string ToRazor()
        {
            var firstNonWhiteSpaceChar = Regex.Match(_templateLine, @"[^\s\\]").Groups[0].Value;
            var indexOfFirstNonWhiteSpaceChar = _templateLine.IndexOf(firstNonWhiteSpaceChar, StringComparison.Ordinal);
            _leadingWhiteSpace = _templateLine.Substring(0, indexOfFirstNonWhiteSpaceChar);
            _tagKey = Regex.Match(_templateLine, @"{{(.*)}}", RegexOptions.IgnoreCase).Groups[1].Value;

            _firstCharOfTag = _tagKey.First();
            _tagKey = _tagKey.Substring(1, _tagKey.Length - 1).Trim();

            var razor = string.Empty;
            var razorPropertyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_tagKey);

            if (EndSectionToken.Equals(_firstCharOfTag)) LeaveCurrentScope();
            if (EndSectionToken.Equals(_firstCharOfTag)) return RazorCloseBlock();

            var dataModel = new DataModel(_dataAccessScope, _dataModel);
            _type = dataModel.TypeForTagKey(_tagKey);

            var scopeName = _formattingScopePresentation.ScopeNameCorrectedForRendering();

            razor = _formattingScopePresentation.ApplyScopeMarker(razor);

            switch (_type)
            {
                case JTokenType.Boolean:
                    razor = InvertedSectionToken.Equals(_firstCharOfTag)
                        ? $"{_leadingWhiteSpace}{razor}{string.Format(RazorFalseyIf, scopeName, razorPropertyName, _leadingWhiteSpace)}"
                        : $"{_leadingWhiteSpace}{razor}{string.Format(RazorTruthyIf, scopeName, razorPropertyName, _leadingWhiteSpace)}";
                    break;
                case JTokenType.Array:
                    var propertyNameSingular = _formattingScopePresentation.PluralToSingularName(razorPropertyName);
                    razor =
                        $"{_leadingWhiteSpace}{razor}{string.Format(RazorForEach, propertyNameSingular, scopeName, razorPropertyName, _leadingWhiteSpace)}";
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
            => $"{_leadingWhiteSpace}{RazorCloseBlockToken}";
    }
}