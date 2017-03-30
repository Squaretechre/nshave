using NShave.Mustache;

namespace NShave.Scope
{
    public class ScopeType
    {
        public readonly string Name;
        private readonly TokenType _type;

        public ScopeType(string name, TokenType type)
        {
            Name = name;
            _type = type;
        }

        public string ApplyScopeMarkerTo(string razorLine)
        {
            var lineWithScopeMarker = razorLine;
            switch (_type)
            {
                case TokenType.HtmlUnorderedList:
                case TokenType.Default:
                    lineWithScopeMarker = $"@{razorLine}";
                    break;
                default:
                    lineWithScopeMarker = razorLine;
                    break;
            }
            return lineWithScopeMarker;
        }

        public bool IsPresentational()
        {
            return _type.Equals(TokenType.HtmlUnorderedList);
        }
    }
}