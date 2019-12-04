using System;
using System.Threading.Tasks;
using System.Linq;
using PickAll.Tests.Fakes;

namespace PickAll.Tests
{
    static class Utilities
    {
        public static async Task<int> GetFakeSearcherResultsCount<T>()
        {
            var searcher = (Searcher)Activator.CreateInstance(typeof(T));
            searcher.Context = new EmptyBrowsingContext();
            var results = await searcher.Search("nothing");
            return results.Count();
        }

        public static string GetRandomDescription<T>()
        {
            var searcher = (Searcher)Activator.CreateInstance(typeof(T));
            searcher.Context = new EmptyBrowsingContext();
            var results = searcher.Search("nothing").GetAwaiter().GetResult();
            var index = new Random().Next(results.Count() - 1);
            return results.ElementAt(index).Description;
        }
    }
}