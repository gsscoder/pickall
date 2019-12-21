using System;

namespace PickAll.Internal
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

        public static bool IsService(this Type type)
        {
            return IsSearcher(type) || IsPostProcessor(type);
        }
    }
}