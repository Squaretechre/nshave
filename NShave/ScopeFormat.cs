using System;
using System.Linq;

namespace NShave
{
    public class ScopeFormat
    {
        private const string TabIndentation = "    ";
        private const string RazorBeginBlockInGlobalScopeMarker = "@";
        private const string RazorAlreadyInBlockScopeMarker = "";
        private readonly Scope _scope;

        public ScopeFormat(Scope scope)
        {
            _scope = scope;
        }

        public string Indentation()
            => string.Concat(Enumerable.Repeat(TabIndentation, _scope.Nesting()));

        public string ScopeMarker()
            => _scope.IsDefault()
                ? RazorBeginBlockInGlobalScopeMarker
                : RazorAlreadyInBlockScopeMarker;

        public string ScopeNameCorrectedForRendering()
            => _scope.IsDefault()
                ? _scope.Current()
                : PluralToSingularName(_scope.Current());

        public string PluralToSingularName(string razorPropertyName)
            => razorPropertyName
                .Substring(0, razorPropertyName.Length - 1)
                .Insert(0, razorPropertyName.ToLower().First().ToString())
                .Remove(1, 1);

        public string NewLine() => Environment.NewLine;
    }
}