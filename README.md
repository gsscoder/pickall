[![Build Status](https://dev.azure.com/gsscoder/pickall/_apis/build/status/gsscoder.pickall?branchName=master)](https://dev.azure.com/gsscoder/pickall/_build/latest?definitionId=2&branchName=master)
[![NuGet](https://img.shields.io/nuget/dt/pickall.svg)](https://nuget.org/packages/pickall)
[![NuGet](https://img.shields.io/nuget/vpre/pickall.svg)](https://www.nuget.org/packages/pickall)
[![Join the Gitter chat!](https://badges.gitter.im/gsscoder/pickall.svg)](https://gitter.im/pickallwebsearcher/community#)

# PickAll

![alt text](/assets/icon.png "SharpX Logo")

.NET agile and extensible web searching API. Built with [AngleSharp](https://anglesharp.github.io/).

## Philosophy

PickAll is primarily designed to collect a limited amount of results (possibly the more relavant) from different sources and process these in a chain of steps. Results are essentially URLs and descriptions, but more data can be handled.

## Documentation

Documentation is available in the project [wiki](https://github.com/gsscoder/pickall/wiki).

## Targets

- .NET Standard 2.0
- .NET Core 3.1
- .NET 5.0

## Install via NuGet

```sh
$ dotnet add package PickAll --version 1.3.1
  Determining projects to restore...
  ...
```

## Build and sample

```sh
# clone the repository
$ git clone https://github.com/gsscoder/pickall.git

# build the package
$ cd pickall/src/PickAll
$ dotnet build -c release

# execute sample
$ cd pickall/samples/PickAll.Sample
$ dotnet build -c release
$ cd ../../artifacts/PickAll.Sample/Release/netcoreapp3.0/PickAll.Sample
./PickAll.Sample "Steve Jobs" -e bing:duckduckgo
Searching 'Steve Jobs' ...
[0] Bing: "Steve Jobs - Wikipedia": "https://it.wikipedia.org/wiki/Steve_Jobs"
[0] DuckDuckGo: "Steve Jobs - Wikipedia": "https://en.wikipedia.org/wiki/Steve_Jobs"
[1] DuckDuckGo: "Steve Jobs - Apple, Family & Death - Biography": "https://www.biography.com/business-figure/steve-jobs"
[2] Bing: "CC-BY-SA licenza": "http://creativecommons.org/licenses/by-sa/3.0/"
[2] DuckDuckGo: "Steve Jobs - IMDb": "https://www.imdb.com/name/nm0423418/"
[3] Bing: "Biografia di Steve Jobs - Biografieonline": "https://biografieonline.it/biografia.htm?BioID=1560&biografia=Steve+Jobs"
```

## Test

```sh
# change to tests directory
$ cd pickall/tests/PickAll.Specs

# build with debug configuration
$ dotnet build -c debug
...

# execute tests
$ dotnet test
...
```

## At a glance

**CSharp:**
```csharp
using PickAll;

var context = new SearchContext()
    .WithEvents()
    .With<Google>() // search on google.com
    .With<Yahoo>() // search on yahoo.com
    .With<Uniqueness>() // remove duplicates
    .With<Order>() // prioritize results
    // match Levenshtein distance with maximum of 15
    .With<FuzzyMatch>(new FuzzyMatchSettings { Text = "mechanics", MaximumDistance = 15 });
    // repeat a search using more frequent words of previous results
    .With<Improve>(new ImproveSettings { WordCount = 2, NoiseLength = 3 })
    // scrape result pages and extract all text
    .With<Textify>(new TextifySettings { IncludeTitle = true, NoiseLength = 3 });
// attach events
context.ResultCreated += (sender, e) => Console.WriteLine($"Result created from {e.Result.Originator}");
// execute services (order of addition)
var results = await context.SearchAsync("quantum physics");
// do anything you need with LINQ
var scientific = results.Where(result => result.Url.Contains("wikipedia"));
foreach (var result in scientific) {
    Console.WriteLine($"{result.Url} {result.Description}");
}
```

**FSharp:**
```fsharp
let context = new SearchContext(typeof<Google>,
                                typeof<DuckDuckGo>,
                                typeof<Yahoo>)
let results = context.SearchAsync("quantum physics")
              |> Async.AwaitTask
              |> Async.RunSynchronously

results |> Seq.iter (fun x -> printfn "%s %s" x.Url x.Description)
```

## Libraries

- [AngleSharp](https://github.com/AngleSharp/AngleSharp)
- [AngleSharp.Io](https://github.com/AngleSharp/AngleSharp.Io)
- [SharpX](https://github.com/gsscoder/sharpx)
- [CommandLineParser](https://github.com/commandlineparser/commandline)
- [xUnit.net](https://github.com/xunit/xunit)
- [FluentAssertions](https://github.com/fluentassertions/fluentassertions)
- [WaffleGenerator](https://github.com/SimonCropp/WaffleGenerator)
- [Bogus](https://github.com/bchavez/Bogus)

## Tools

- [Paket](https://github.com/fsprojects/Paket)

## Icon

- [Search Engine](https://thenounproject.com/search/?q=search%20engine&i=2054907) icon designed by Vectors Market from [The Noun Project](https://thenounproject.com/).
