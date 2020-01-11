using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickAll.Tests.Fakes
{
    struct ArbitrarySearcherSettings
    {
        public ushort Samples { get; set; }

        public bool Random { get; set; }
    }

    class ArbitrarySearcher : Searcher
    {
        private readonly ArbitrarySearcherSettings _settings;

        public ArbitrarySearcher(object settings, RuntimePolicy policy) : base(settings, policy)  
        {
            if (!(Settings is ArbitrarySearcherSettings)) {
                throw new NotSupportedException();
            }
            _settings = (ArbitrarySearcherSettings)Settings;
        }

        public override async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
        {
            return await Task.Run(() => _());
            IEnumerable<ResultInfo> _() {
                var originator = Guid.NewGuid().ToString();
                var results = _settings.Random
                    ? ResultInfoBuilder.GenerateRandom(originator, _settings.Samples)
                    : ResultInfoBuilder.Generate(originator, _settings.Samples);
                if (Policy.MaximumResults.HasValue) {
                    results = results.Take((int)Policy.MaximumResults.Value);
                }
                return results;
            }
        }
    }
}