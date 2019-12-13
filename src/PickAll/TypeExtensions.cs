using System;

namespace PickAll
{
    static class TypeExtensions
    {
        public static bool IsSearcher(this Type type)
        {
            return type.IsSubclassOf(typeof(Searcher)); 
        }
        
        public static bool IsPostProcessor(this Type type)
        {
            return type.IsSubclassOf(typeof(PostProcessor)); 
        }
    }
}