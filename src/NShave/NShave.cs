using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NShave.Mustache;
using NShave.Scope;

namespace NShave
{
    public class NShave
    {
        public string ToRazor(string mustacheTemplate, string data)
        {
            var dataAccessScope = new ScopeDataModel();
            var formattingScope = new ScopeDataModel();
            var formatting = new ScopePresentation(dataAccessScope, formattingScope);
            var model = (JObject)JsonConvert.DeserializeObject(data);
            return new MustacheDocument(mustacheTemplate, model, dataAccessScope, formatting).ToRazor();
        }
    }
}