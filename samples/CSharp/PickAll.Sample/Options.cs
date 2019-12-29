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

        [Option('f', "fuzzy", HelpText = "Fuzzy matching of Levenshtein distance 0-10")]
        public string FuzzyMatch { get; set; }

        [Option('i', "improve", HelpText = "Enable improve search post processor")]
        public bool Improve { get; set; }

        [Option('w', "wordify", HelpText = "Enable improve wordify post processor")]
        public bool Wordify { get; set; }       

        [Option('e', "engines", HelpText = "Search engines to use separated by ':'",
            Separator = ':')]
        public IEnumerable<string> Engines { get; set; }
    }
}