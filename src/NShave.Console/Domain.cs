using System;

namespace NShave.Console
{
    public class Domain
    {
        private readonly IDomainAdapter _adapter;

        public Domain(IDomainAdapter adapter)
        {
            _adapter = adapter;
        }

        public void ToRazor(Action<string> conversionCompleteCallback)
        {
            var nshave = new NShave();
            var convertedRazor = nshave.ToRazor(_adapter.MustacheTemplate, _adapter.Data);
            conversionCompleteCallback(convertedRazor);
        }
    }
}