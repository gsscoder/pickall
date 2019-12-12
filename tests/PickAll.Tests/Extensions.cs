using System;
using System.Collections.Generic;
using System.Linq;

namespace PickAll.Tests
{
    static class Extensions
    {
        public static T Second<T>(this IEnumerable<T> collection)
        {
            return collection.ElementAt(1);
        }

        public static T Random<T>(this T[] array)
        {
            var index = new Random().Next(array.Length - 1);
            return array[index];
        }

        public static T Random<T>(this IEnumerable<T> collection)
        {
            var index = new Random().Next(collection.Count() - 1);
            return collection.ElementAt(index);
        }

        public static ResultInfo WithIndex(this ResultInfo result, ushort index)
        {
            return new ResultInfo(
                result.Originator, index, result.Url, result.Description, result.Data);
        }

        public static IEnumerable<ResultInfo> Search(this Searcher searcher,
            string query = "none")
        {
            return searcher.SearchAsync(query).GetAwaiter().GetResult();
        }

        public static IEnumerable<ResultInfo> Search(this SearchContext context,
            string query = "none")
        {
            return context.SearchAsync(query).GetAwaiter().GetResult();
        }
    }
}