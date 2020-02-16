using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpx;
using PickAll;

class ArbitrarySearcherSettings
{
    public ArbitrarySearcherSettings()
    {
        AtLeast = Maybe.Nothing<ushort>();
    }

    public ushort Samples { get; set; }

    public Maybe<ushort> AtLeast { get; set; }
}

class ArbitrarySearcher : Searcher
{
    readonly ArbitrarySearcherSettings _settings;

    public ArbitrarySearcher(object settings) : base(settings)  
    {
        if (!(Settings is ArbitrarySearcherSettings)) {
            throw new NotSupportedException();
        }
        _settings = (ArbitrarySearcherSettings)Settings;
    }

    public override Task<IEnumerable<ResultInfo>> SearchAsync(string query)
    {
        return Task.FromResult(_()); IEnumerable<ResultInfo> _()
        {
            var originator = Guid.NewGuid().ToString();
            var results = _settings.AtLeast.IsJust()
                ? ResultInfoBuilder.GenerateRandom(originator,
                    _settings.AtLeast.FromJust<ushort>(@default: 1), _settings.Samples)
                : ResultInfoBuilder.Generate(originator, _settings.Samples);
            if (Runtime.MaximumResults.HasValue) {
                results = results.Take((int)Runtime.MaximumResults.Value);
            }
            for (ushort i = 0; i < results.Count(); i++) {
                var result = results.ElementAt(i);
                yield return CreateResult(i, result.Url, result.Description);
            }
        }
    }
}