using System;
using System.Threading.Tasks;
using System.Linq;
using PickAll.Tests.Fakes;

namespace PickAll.Tests
{
    static class Utilities
    {
        public static string RandomDescriptionOf<T>() where T : Searcher
        {
            return SearcherFor<T, string>(searcher => {
                var results = searcher.SearchSync();
                var index = new Random().Next(results.Count() - 1);
                return results.ElementAt(index).Description;
                });
        }

         public static int ResultsCountOf<T>() where T : Searcher
        {
            return Utilities.SearcherFor<T, int>(searcher => searcher.SearchSync().Count());
        }

        public static TResult SearcherFor<T, TResult>(Func<T, TResult> selector) where T : Searcher
        {
            var searcher = (T)Activator.CreateInstance(typeof(T));
            (searcher as Searcher).Context = new EmptyBrowsingContext();
            return selector(searcher);
        }


    }
}