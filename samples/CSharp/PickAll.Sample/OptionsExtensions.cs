using System;
using System.Linq;
using PickAll;

static class OptionsExtensions
{
    public static SearchContext ToContext(this Options options)
    {
        SearchContext context;
        if (!options.Engines.Any()) {
            context = SearchContext.Default;
        }
        else {
            context = new SearchContext();
            foreach (var engine in options.Engines) {
                context = context.With(engine);
            }
            context = context
                .With<Uniqueness>()
                .With<Order>();
        }
        if (options.Timeout.HasValue) {
            context = context.WithConfiguration(
                new ContextSettings { Timeout = TimeSpan.FromSeconds(options.Timeout.Value) });
        }
        if (!string.IsNullOrEmpty(options.FuzzyMatch)) {
            context = context.With<FuzzyMatch>(
                new FuzzyMatchSettings {
                    Text = options.FuzzyMatch,
                    MaximumDistance = 10 });
        }
        if (options.Improve) {
            context = context.With<Improve>(
                new ImproveSettings {
                    WordCount = 2,
                    NoiseLength = 3});
        }
        if (options.Wordify) {
            context = context.With<Wordify>(
                new WordifySettings {
                    IncludeTitle = true,
                    NoiseLength = 3});
        }
        return context;
    }
}