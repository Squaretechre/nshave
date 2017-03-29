using System;
using System.Linq;

namespace NShave
{
    public class ScopeFormat : IEnterScope, ILeaveScope
    {
        private const string TabIndentation = "    ";
        private const string RazorBeginBlockInGlobalScopeMarker = "@";
        private const string RazorAlreadyInBlockScopeMarker = "";
        private readonly Scope _dataAccessScope;
        private readonly Scope _formattingScope;

        public ScopeFormat(Scope dataAccessScope, Scope formattingScope)
        {
            _dataAccessScope = dataAccessScope;
            _formattingScope = formattingScope;
        }

        public string Indentation()
            => string.Concat(Enumerable.Repeat(TabIndentation, _formattingScope.Nesting()));

        public string ScopeMarker()
            => _formattingScope.IsDefault()
                ? RazorBeginBlockInGlobalScopeMarker
                : RazorAlreadyInBlockScopeMarker;

        public string ScopeNameCorrectedForRendering()
            => _formattingScope.IsDefault()
                ? _dataAccessScope.Current()
                : PluralToSingularName(_dataAccessScope.Current());

        public string PluralToSingularName(string razorPropertyName)
        {
            var propertyName = FirstCharToLower(razorPropertyName);
            propertyName = razorPropertyName.EndsWith("ies") 
                ? SingularSpellingForIesEnding(propertyName) 
                : SingularSpellingForSEnding(propertyName);

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
        public void Enter(string scopeName) => _formattingScope.Enter(scopeName);
        public void Leave(string scopeName) => _formattingScope.Leave(scopeName);
    }
}