using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PickAll.Searchers;
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
                // Exclude Facebook searcher for custom configuration
                var engines = options.Engines.Where(engine => engine != "Facebook");
                context = new SearchContext();
                foreach (var engine in engines) {
                    context = context.With(engine);
                }
                context = context
                    .With<Uniqueness>()
                    .With<Order>();
            }
            if (!string.IsNullOrEmpty(options.FuzzyMatch)) {
                context = context.With<FuzzyMatch>(
                    new FuzzyMatchSettings {
                        Text = options.FuzzyMatch,
                        MaximumDistance = 10 });
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