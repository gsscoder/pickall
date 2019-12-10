using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickAll.Tests.Fakes
{
    public class Post_processor_marker_settings
    {
        public string Stamp;
    }

    public class Post_processor_marker : PostProcessor
    {
        private readonly Post_processor_marker_settings _settings;

        public Post_processor_marker(object settings) : base(settings)
        {
            _settings = Settings as Post_processor_marker_settings;
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