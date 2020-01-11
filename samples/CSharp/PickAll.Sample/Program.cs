using System;
using System.Linq;
using System.Threading.Tasks;
using PickAll.PostProcessors;
using CommandLine;

namespace PickAll.Simple
{
    class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(options => DoSearch(options).GetAwaiter().GetResult(),
                _ => 1);
        }

        static async Task<int> DoSearch(Options options)
        {
            SearchContext context;
            if (options.Engines.Count() == 0) {
                context = SearchContext.Default;
            } else {
                context = new SearchContext();
                foreach (var engine in options.Engines) {
                    context = context.With(engine);
                }
                context = context
                    .With<Uniqueness>()
                    .With<Order>();
            }
            if (options.Timeout != null) {
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

            var results = await context.SearchAsync(options.Query);
            foreach (var result in results) {
                Console.WriteLine(
                    $"[{result.Index}] {result.Originator}: \"{result.Description}\": \"{result.Url}\"");
                if (result.Data != null) {
                    Console.WriteLine($"  Data:\n    {result.Data}");
                }
            }

            return 0;
        }
    }
}