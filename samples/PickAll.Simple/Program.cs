using System;
using System.Threading.Tasks;

namespace PickAll.Simple
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var context = SearchContext.Default();
            var results = await context.Search(".net core");

            foreach(var result in results)
            {
                Console.WriteLine(
                    $"[{result.Index}] {result.Originator.ToUpper()}: \"{result.Description}\": \"{result.Url}\"");
            }
        }
    }
}