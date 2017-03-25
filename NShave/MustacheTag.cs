using Newtonsoft.Json.Linq;

namespace NShave
{
    public class MustacheTag
    {
        private readonly string _name;
        private readonly JTokenType _type;

        public MustacheTag(string name, JTokenType type)
        {
            _name = name;
            _type = type;
        }

        public string ToRazor()
        {
            var razor = string.Empty;
            switch (_type)
            {
                case JTokenType.Boolean:
                    razor = $"if (@Model.{_name}) {{";
                    break;
                case JTokenType.Array:
                    var singularName = _name.Substring(0, _name.Length - 1);
                    razor = $"foreach (var {singularName} in Model.{_name}) {{";
                    break;
            }
            return razor;
        }
    }
}