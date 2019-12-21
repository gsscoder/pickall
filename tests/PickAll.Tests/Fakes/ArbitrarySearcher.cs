using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickAll.Tests.Fakes
{
    class ArbitrarySearcherSettings
    {
        public ushort Samples;
    }

    class ArbitrarySearcher : Searcher
    {
        private readonly ArbitrarySearcherSettings _settings;

        public ArbitrarySearcher(
            SearchContext context, object settings) : base(context, settings)  
        {
            _settings = Settings as ArbitrarySearcherSettings;
            if (_settings == null) {
                throw new NotSupportedException();
            }
        }

        public override async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
        {
            return await Task.Run(() => _());
            IEnumerable<ResultInfo> _() {
                var originator = Guid.NewGuid().ToString();
                return ResultInfoGenerator.Generate(originator, _settings.Samples);
            }
        }
    }
}