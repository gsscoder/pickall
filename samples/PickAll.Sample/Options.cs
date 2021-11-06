using CommandLine;
using System.Collections.Generic;

sealed class Options
{
    [Value(0, MetaName = "search query", HelpText = "Query to submit to search engines",
            Required = true)]
    public string Query { get; set; }

    [Option("timeout", HelpText = "Maximum timeout for HTTP requests in seconds")]
    public uint? Timeout { get; set; }

    [Option('f', "fuzzy", HelpText = "Fuzzy matching of Levenshtein distance 0-10")]
    public string FuzzyMatch { get; set; }

    [Option('i', "improve", HelpText = "Enable improve search post processor")]
    public bool Improve { get; set; }

    [Option('t', "textify", HelpText = "Enable textify post processor")]
    public bool Wordify { get; set; }       

    [Option('e', "engines", HelpText = "Search engines to use separated by ':'",
        Separator = ':')]
    public IEnumerable<string> Engines { get; set; }
}
