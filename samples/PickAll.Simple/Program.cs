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
            var context = SearchContext.Default();
            if (!string.IsNullOrEmpty(options.FuzzyMatch)) {
                context = context.With(new FuzzyMatch(options.FuzzyMatch, 10));
            }
            var results = await context.SearchAsync(options.Query);
            var filtered = results;

            foreach(var result in filtered) {
                Console.WriteLine(
                    $"[{result.Index}] {result.Originator}: \"{result.Description}\": \"{result.Url}\"");
            }

            return 0;
        }
    }
}