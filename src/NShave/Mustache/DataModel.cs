using Newtonsoft.Json.Linq;
using NShave.Scope;

namespace NShave.Mustache
{
    public class DataModel
    {
        private readonly IScope _dataAccessScope;
        private readonly JObject _dataModel;

        public DataModel(IScope dataAccessScope, JObject dataModel)
        {
            _dataAccessScope = dataAccessScope;
            _dataModel = dataModel;
        }

        public JTokenType TypeForTagKey(string tagKey)
        {
            var resolvedToken = _dataAccessScope.IsDefault()
                ? _dataModel[tagKey]
                : _dataModel.SelectToken(_dataAccessScope.AsJsonPath())[tagKey];

            return resolvedToken.Type.Equals(JTokenType.Object) 
                ? JTokenType.Boolean 
                : resolvedToken.Type;
        }
    }
}