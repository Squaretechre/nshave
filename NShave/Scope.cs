using System.Collections.Generic;

namespace NShave
{
    public class Scope
    {
        private readonly Stack<string> _scope;
        private const string DefaultScopeName = "Model";

        public Scope()
        {
            _scope = new Stack<string>();
            _scope.Push(DefaultScopeName);
        }

        public string Current() => _scope.Peek();

        public void Enter(string scopeName) => _scope.Push(scopeName);

        public void Leave(string scopeName)
        {
            if (_scope.Peek().Equals(scopeName)) _scope.Pop();
        }
    }
}