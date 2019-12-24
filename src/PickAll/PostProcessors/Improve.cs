using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace PickAll.PostProcessors
{
    public class ImproveSettings
    {
        public ushort WordCount;

        public ushort NoiseLength;
    }

    public class Improve : PostProcessor
    {
        private readonly ImproveSettings _settings;

        public Improve(SearchContext context, object settings) : base(context, settings)
        {
            _settings = settings as ImproveSettings;
            if (_settings == null) {
                throw new NotSupportedException(
                    $"{nameof(settings)} must be of ImproveSettings type");
            }
        }

        internal IEnumerable<string> FoldDescriptions(IEnumerable<ResultInfo> results)
        {
            var words = from result in results
                        from word in result.Description.Split()
                        where word.IsAlphanumeric()
                        select word;
            var folded =  from w in
                              from word in words
                              group word by word into g
                              select new Tuple<string, int>(g.Key, g.Count())
                          orderby w.Item2 descending
                          select w;
            IEnumerable<Tuple<string, int>> refined;
                var query = Context.Query ?? string.Empty;
                var queryWords = query.ToLower().Split();
                refined = from computed in folded
                          where !queryWords.Contains(computed.Item1.ToLower())
                          && !CouldBeNoise(computed.Item1)
                          select computed;

            return (from computed in refined
                    select computed.Item1).Take(_settings.WordCount);
        } 

        public override IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
        {
            var builder = new StringBuilder();
            builder.Append(string.Join(" ", FoldDescriptions(results).ToArray()));
            builder.Append(' ');
            builder.Append(Context.Query);

            return Context
                       .WithoutAll<PostProcessor>()
                       .With<Uniqueness>()
                       .With<Order>()
                       .SearchAsync(builder.ToString()).GetAwaiter().GetResult();
        }

        bool CouldBeNoise(string word)
        {
            if (_settings.NoiseLength == 0) {
                return false;
            }
            return word.Length <= _settings.NoiseLength;
        }
    }
}