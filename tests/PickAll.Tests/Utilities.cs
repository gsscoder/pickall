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
    }
}