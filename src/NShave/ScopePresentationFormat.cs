using System;
using System.Linq;

namespace NShave
{
    public class ScopePresentationFormat : IEnterScope, ILeaveScope
    {
        private const string TabIndentation = "    ";
        private readonly Scope _dataAccessScope;
        private readonly Scope _formattingScope;

        public ScopePresentationFormat(Scope dataAccessScope, Scope formattingScope)
        {
            _dataAccessScope = dataAccessScope;
            _formattingScope = formattingScope;
        }

        public string Indentation()
            => string.Concat(Enumerable.Repeat(TabIndentation, _formattingScope.Nesting()));

        public string ApplyScopeMarker(string razorLine)
            => _formattingScope.Current().ApplyScopeMarkerTo(razorLine);

        public string ScopeNameCorrectedForRendering()
            => _formattingScope.IsDefault()
                ? _dataAccessScope.Current().Name
                : PluralToSingularName(_dataAccessScope.Current().Name);

        public string PluralToSingularName(string razorPropertyName)
        {
            var propertyName = FirstCharToLower(razorPropertyName);
            if (razorPropertyName.EndsWith("ies"))
            {
                propertyName = SingularSpellingForIesEnding(propertyName);
            } 
            else if (razorPropertyName.EndsWith("s"))
            {
                propertyName = SingularSpellingForSEnding(propertyName);
            }

            return propertyName;
        }

        private static string SingularSpellingForSEnding(string propertyName)
            => propertyName.Substring(0, propertyName.Length - 1);

        private static string FirstCharToLower(string razorPropertyName)
            => razorPropertyName
                .Insert(0, razorPropertyName.ToLower().First().ToString())
                .Remove(1, 1);

        private static string SingularSpellingForIesEnding(string propertyName)
        {
            var singularSpelling = propertyName.Substring(0, propertyName.Length - 3);
            return $"{singularSpelling}y";
        }

        public string NewLine() => Environment.NewLine;
        public void Enter(ScopeType scope) => _formattingScope.Enter(scope);
        public void Leave(string scopeName) => _formattingScope.Leave(scopeName);
    }
}