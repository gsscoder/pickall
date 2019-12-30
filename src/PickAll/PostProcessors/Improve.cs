using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace PickAll.PostProcessors
{
    /// <summary>
    /// Settings for <see cref="Improve"/> post processor.
    /// </summary>
    public class ImproveSettings
    {
        /// <summary>
        /// Number of word with highest frequency to use in subsequent search.
        /// </summary>
        public ushort WordCount;

        /// <summary>
        /// Length of words to be considered noise.
        /// </summary>
        public ushort NoiseLength;
    }

    /// <summary>
    /// Improves results computing word frequency to perform a subsequent search.
    /// </summary>
    public class Improve : PostProcessor
    {
        private readonly ImproveSettings _settings;

        public Improve(object settings) : base(settings)
        {
            _settings = settings as ImproveSettings;
            if (_settings == null) {
                throw new NotSupportedException(
                    $"{nameof(settings)} must be of ImproveSettings type");
            }
        }

#if DEBUG
        internal
#endif
        IEnumerable<string> FoldDescriptions(IEnumerable<ResultInfo> results)
        {
            Func<string, bool> couldBeNoise = _settings.NoiseLength == 0
                ? couldBeNoise =  _ => false
                : w => w.Length <= _settings.NoiseLength;
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
                          && !couldBeNoise.Invoke(computed.Item1)
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
    }
}