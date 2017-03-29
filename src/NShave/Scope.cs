using System.Collections.Generic;
using System.Linq;

namespace NShave
{
    public class Scope : IScope, IEnterScope, ILeaveScope
    {
        private const string DefaultScopeName = "Model";
        private readonly Stack<string> _scope;

        public Scope()
        {
            _scope = new Stack<string>();
            _scope.Push(DefaultScopeName);
        }

        public void Enter(string scopeName) => _scope.Push(scopeName);

        public void Leave(string scopeName)
        {
            if (_scope.Peek().Equals(scopeName)) _scope.Pop();
        }

        public bool IsDefault() => _scope.Peek().Equals(DefaultScopeName);

        public string Current() => _scope.Peek();

        public int Nesting() => _scope.Count - 1;

        public string AsJsonPath() => $"{CreatePathFromScopeStack()}[0]";

        private string CreatePathFromScopeStack() => 
            _scope
            .Reverse()
            .Skip(1)
            .Aggregate((current, next) => $"{current}[0].{next}");
    }
}