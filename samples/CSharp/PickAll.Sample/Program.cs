using System;
using System.Linq;
using System.Threading.Tasks;
using PickAll;
using CommandLine;

sealed class Program
{
    const int exitOK = 0;
    const int exitFail = 1;

    static int Main(string[] args)
    {
        return Parser.Default.ParseArguments<Options>(args)
            .MapResult(options => DoSearch(options).RunSynchronously<int>(),
            _ => exitFail);
    }

    static async Task<int> DoSearch(Options options)
    {
        var context = options.ToContext();
        var results = await context.SearchAsync(options.Query);
        foreach (var result in results) {
            Console.WriteLine(
                $"[{result.Index}] {result.Originator}: \"{result.Description}\": \"{result.Url}\"");
            if (result.Data != null) {
                Console.WriteLine($"  Data:\n    {result.Data}");
            }
        }
        return exitOK;
    }
}