using System;
using System.Linq;
using System.Threading.Tasks;
using PickAll.PostProcessors;

namespace PickAll.Simple
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var context = SearchContext.Default()
                .With(new FuzzyMatch("Steve Jobs Biography", 10));
            var results = await context.Search("steve jobs");
            var filtered = results;

            foreach(var result in filtered)
            {
                Console.WriteLine(
                    $"[{result.Index}] {result.Originator.ToUpper()}: \"{result.Description}\": \"{result.Url}\"");
            }
        }
    }
}