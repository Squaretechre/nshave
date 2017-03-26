using System.Collections.Generic;

namespace NShave
{
    public class Scope : IScope, IEnterScope, ILeaveScope
    {
        private readonly Stack<string> _scope;
        private const string DefaultScopeName = "Model";

        public Scope()
        {
            _scope = new Stack<string>();
            _scope.Push(DefaultScopeName);
        }

        public bool IsDefault() => _scope.Peek().Equals(DefaultScopeName);

        public string Current() => _scope.Peek();

        public int Nesting() => _scope.Count - 1;

        public void Enter(string scopeName) => _scope.Push(scopeName);

        public void Leave(string scopeName)
        {
            if (_scope.Peek().Equals(scopeName)) _scope.Pop();
        }
    }
}