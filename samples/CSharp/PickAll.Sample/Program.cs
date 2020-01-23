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
        context.ResultCreated += (sender, e) => {
            var result = e.Result;
            Console.WriteLine(
                $"[{result.Index}] {result.Originator}: \"{result.Description}\": \"{result.Url}\"");
            if (result.Data != null) {
                Console.WriteLine($"  Data:\n    {result.Data}");
            }
        };

        var results = await context.SearchAsync(options.Query);
        return exitOK;
    }
}