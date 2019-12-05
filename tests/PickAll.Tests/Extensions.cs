using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using PickAll.Tests.Fakes;

namespace PickAll.Tests
{
    static class Extensions
    {
        public static IEnumerable<ResultInfo> SearchSync(this Searcher searcher,
            string query = "none")
        {
            return searcher.Search(query).GetAwaiter().GetResult();
        }

        public static IEnumerable<ResultInfo> SearchSync(this SearchContext context,
            string query = "none")
        {
            return context.Search(query).GetAwaiter().GetResult();
        }
    }
}