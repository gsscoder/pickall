using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using PickAll.Tests.Fakes;

namespace PickAll.Tests
{
    static class Extensions
    {
        public static T Second<T>(this IEnumerable<T> collection)
        {
            return collection.ElementAt(1);
        }

        public static IEnumerable<ResultInfo> SearchSync(this Searcher searcher,
            string query = "none")
        {
            return searcher.SearchAsync(query).GetAwaiter().GetResult();
        }

        public static IEnumerable<ResultInfo> SearchSync(this SearchContext context,
            string query = "none")
        {
            return context.SearchAsync(query).GetAwaiter().GetResult();
        }
    }
}