using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickAll.Tests.Fakes
{
    public class MarkerSettings
    {
        public string Stamp;
    }

    public class Marker : PostProcessor
    {
        private readonly MarkerSettings _settings;

        public Marker(object settings) : base(settings)
        {
            _settings = Settings as MarkerSettings;
            if (_settings == null) {
                throw new NotImplementedException();
            }
        }

        public override async Task<IEnumerable<ResultInfo>> ProcessAsync(IEnumerable<ResultInfo> results)
        {
            return await Task.Run(() => _());
            IEnumerable<ResultInfo> _() {
                foreach (var result in results) {
                    yield return new ResultInfo(result.Originator, result.Index, result.Url,
                        $"{_settings.Stamp}|{result.Description}", null);
                }
            }
        }
    }
}