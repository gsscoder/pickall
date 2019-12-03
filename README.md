# PickAll

.NET agile and extensible web searching API. Built with [AngleSharp](https://github.com/AngleSharp/AngleSharp).

## Build and sample
**NOTE**: .NET Core 3.0 or higher is required.
```sh
# clone the repository
$ git clone https://github.com/gsscoder/pickall.git

# change the working directory
$ cd pickall

# build the package
$ dotnet build -c Release.

# execute sample
$ ./artifacts/PickAll.Samples/Release/netcoreapp3.0/PickAll.Simple 
[0] GOOGLE: "Guida a .NET Core | Microsoft Docs": "https://docs.microsoft.com/it-it/dotnet/core/"
[0] DUCKDUCKGO: ".NET Core Guide | Microsoft Docs": "https://docs.microsoft.com/en-us/dotnet/core/"
[1] GOOGLE: "Introduzione a .NET Core": "https://docs.microsoft.com/it-it/dotnet/core/get-started"
[2] GOOGLE: "Informazioni su .NET Core": "https://docs.microsoft.com/it-it/dotnet/core/about"
[2] DUCKDUCKGO: "About .NET Core | Microsoft Docs": "https://docs.microsoft.com/en-us/dotnet/core/about"
[3] GOOGLE: "Compilare un'applicazione ...": "https://docs.microsoft.com/it-it/dotnet/core/tutorials/with-visual-studio"
...
```

# Test
```sh
# change to tests directory
$ cd pickall/tests/PickAll.Tests

# build with debug configuration
$ dotnet build -c Debug
...

# execute tests
$ dotnet test
...
```

## At a glance
```csharp
using PickAll;

var context = new SearchContext()
    .With<GoogleSearcher>() // search on google.com
    .With<DuckDuckGoSearcher>() // search on duckduckgo.com
    .With<UniquenessPostProcessor>() // remove duplicates
    ,With<OrderPostProcessor>(); // order results by index
var results = context.Search("hello web");

foreach (var result in results) {
    Console.WriteLine($"{result.Url}");
}
```

### Notes
- This is a pre-release, since it's under development API can change until stable version.