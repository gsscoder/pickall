using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;

namespace PickAll.Tests
{
    class ArbitrarySearcherSettings
    {
        public bool Unique;

        public ushort Samples;
    }

    class ArbitrarySearcher : Searcher
    {
        private readonly ArbitrarySearcherSettings _settings;

        public ArbitrarySearcher(
            IBrowsingContext context, object settings = null) : base(context, settings)  
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
                if (_settings.Unique) {
                    return ResultInfoGenerator.GenerateUnique(originator, _settings.Samples);
                }
                return ResultInfoGenerator.Generate(originator, _settings.Samples);
            }
        }
    }
}