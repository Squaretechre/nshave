using System.Collections.Generic;
using System.Linq;

namespace NShave
{
    public class Scope : IScope, IEnterScope, ILeaveScope
    {
        private readonly ScopeType _defaultScope = new ScopeType("Model", TokenType.Default);
        private readonly Stack<ScopeType> _scope;

        public Scope()
        {
            _scope = new Stack<ScopeType>();
            _scope.Push(_defaultScope);
        }

        public void Enter(ScopeType scope) => _scope.Push(scope);

        public void Leave(string scopeName)
        {
            if (_scope.Peek().Name.Equals(scopeName)) _scope.Pop();
        }

        public bool IsDefault() => _scope.Peek().Equals(_defaultScope);

        public ScopeType Current() => _scope.Peek();

        public int Nesting() => (_scope.Count - 1) - _scope.Count(s => s.IsPresentational());

        public string AsJsonPath() => $"{CreatePathFromScopeStack()}[0]";

        private string CreatePathFromScopeStack() => 
            _scope
            .Reverse()
            .Skip(1)
            .Select(s => s.Name)
            .Aggregate((current, next) => $"{current}[0].{next}");
    }
}