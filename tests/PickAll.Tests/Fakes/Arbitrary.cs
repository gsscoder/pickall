using System.Collections.Generic;
using System.Linq;

namespace PickAll.Tests
{
    static class Urls
    {
        private static string[] _domains = new string[] {
            "https://www.google.com",
            "https://duckduckgo.com",
            "https://yahoo.com",
            "https://www.facebook.com",
            "https://twitter.com",
            "https://github.com",
            "https://www.reddit.com",
            "https://it.quora.com",
            "https://news.ycombinator.com",
            "https://rome.craigslist.org",
            "https://www.microsoft.com",
            "https://golang.org",
            "https://www.rust-lang.org",
            "https://elm-lang.org",
            "https://www.oracle.com",
            "https://stackoverflow.com",
            "https://unix.stackexchange.com"
        };

        private static string[] _paths = new string[] {
            "/",
            "/index.htm",
            "/index.html",
            "/index.php",
            "/home",
            "/home/index.htm",
            "/hone/index.html",
            "/home/index.php",
            "/home",
            "/search",
            "/contacts",
            "/about",
            "/about/info",
            "/credits",
            "/credits/info.php"
        };

        public static string Generate()
        {
            return $"{_domains.Random()}{_paths.Random()}";
        }
    }

    static class Descriptions
    {
        private static string[] _descriptions = new string[] {
            "Home",
            "Wellcome",
            "Google",
            "Yahoo",
            "DuckDuckGo",
            "Google Translate",
            "Gmail",
            "Search",
            "Advanced Search",
            "Contacts",
            "Info",
            "About",
            "Credits"
        };

        public static string Generate()
        {
            return $"{_descriptions.Random()}";
        }
    }

    static class ResultInfoGenerator
    {
        public static IEnumerable<ResultInfo> Generate(string originator, ushort samples)
        {
            for (ushort index = 0; index <= samples - 1; index++) {
                yield return new ResultInfo(
                    originator, index, Urls.Generate(), Descriptions.Generate(), null);
            } 
        }

        public static IEnumerable<ResultInfo> GenerateUnique(string originator, ushort samples)
        {
            var generated = new List<ResultInfo>();
            for (ushort index = 0; index <= samples - 1; index++) {
                var url = Urls.Generate();
                var description = Descriptions.Generate();
                var searched = from @this in generated
                               where @this.Url == url || @this.Description == description
                               select @this;
                if (searched.Count() == 0) {
                    var result = new ResultInfo(
                        originator, index, url, description, null);
                    generated.Add(result);
                }
                else {
                    index--;
                }
            }
            return generated;
        }
    }
}