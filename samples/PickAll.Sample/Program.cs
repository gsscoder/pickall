using System;
using System.Threading.Tasks;
using CommandLine;

sealed class Program
{
    const int exitOK = 0;
    const int exitFail = 1;

    static int Main(string[] args)
    {
        return Parser.Default.ParseArguments<Options>(args)
            .MapResult(options => ExecuteSearch(options).RunSynchronously<int>(),
            _ => exitFail);
    }

    static async Task<int> ExecuteSearch(Options options)
    {
        var context = options.ToContext();
        context.SearchBegin += (sender, e) => Console.WriteLine($"Searching '{e.Query}' ...");
        var results = await context.SearchAsync(options.Query);
        foreach (var result in results) {
            Console.WriteLine(
                $"[{result.Index}] {result.Originator}: \"{result.Description}\": \"{result.Url}\"");
            if (result.Data != null) {
                Console.WriteLine(
                    $"\tData:\n\t\t{result.Data}");
            }
        }
        return exitOK;
    }
}