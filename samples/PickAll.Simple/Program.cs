using System;
using System.Linq;
using System.Threading.Tasks;

namespace PickAll.Simple
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var context = SearchContext.Default();
            var results = await context.Search("steve jobs");
            var scientific = results.Where(result =>
                result.Description.ToLower().Contains("apple"));

            foreach(var result in scientific)
            {
                Console.WriteLine(
                    $"[{result.Index}] {result.Originator.ToUpper()}: \"{result.Description}\": \"{result.Url}\"");
            }
        }
    }
}