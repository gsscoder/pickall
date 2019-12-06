using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace PickAll.Simple
{
    class Options
    {
        [Value(0, MetaName = "search query", HelpText = "Query to submit to search engines",
                Required = true)]
        public string Query { get; set; }

        [Option('f', "fuzzy", HelpText = "Text for fuzzy matching with Levenshtein distance")]
        public string FuzzyMatch { get; set; }

        [Option('e', "engines", HelpText = "Search engines to use separated by ':'",
            Separator = ':')]
        public IEnumerable<string> Engines { get; set; }
    }
}