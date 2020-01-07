# <img src="/assets/icon.png" height="30px" alt="PickAll Logo"> PickAll

.NET agile and extensible web searching API. Built with [AngleSharp](https://github.com/AngleSharp/AngleSharp).

## Philosophy

PickAll is primarily designed to collect a limited amount of results (possibly the more relavant) from different sources and process these in a chain of steps. Results are essentially URLs and descriptions, but more data can be handled.

## Documentation

An initial draft is available in the project [Wiki](https://github.com/gsscoder/pickall/wiki).

## Install via NuGet

```sh
$ dotnet add package PickAll --version 0.18.4-beta
```

## Build and sample

**Target**: .NET Standard 2.0, .NET Framework 4.6.1
```sh
# clone the repository
$ git clone https://github.com/gsscoder/pickall.git

# build the package
$ cd pickall/src/PickAll
$ dotnet build -c release

# execute sample
$ cd pickall/samples/PickAll.Sample
$ dotnet build -c Release
$ cd ../../artifacts/PickAll.Sample/Release/netcoreapp3.0/PickAll.Sample
./PickAll.Sample "Steve Jobs" -e YAHOO:DuckDuckGo --fuzzy "Steve Jobs Biography"
[0] Yahoo: "Steve Jobs - Wikipedia": "https://it.wikipedia.org/wiki/Steve_Jobs"
[2] DuckDuckGo: "Steve Jobs - IMDb": "https://www.imdb.com/name/nm0423418/"
[8] Yahoo: "Steve Jobs - Wikipedia": "https://en.wikipedia.org/wiki/Steve_Jobs"
[10] Yahoo: "Steve Jobs – Wikipedia": "https://de.wikipedia.org/wiki/Steve_Jobs"
[13] DuckDuckGo: "Steve Jobs - Biography - IMDb": "https://www.imdb.com/name/nm0423418/bio"
[28] DuckDuckGo: "Steve Jobs - Forbes": "https://www.forbes.com/profile/steve-jobs/"
```

## Test

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

**CSharp:**
```csharp
using PickAll;
using PickAll.Searchers;
using PickAll.PostProcessors;

var context = new SearchContext(maximumResults: 30)
    .With<Google>() // search on google.com
    .With<Yahoo>() // search on yahoo.com
    .With<Uniqueness>() // remove duplicates
    .With<Order>() // order results by index
    // match Levenshtein distance with maximum of 15
    .With<FuzzyMatch>(new FuzzyMatchSettings { Text = "mechanics", MaximumDistance = 15 });
    // repeat a search using more frequent words of previous results
    .With<Improve>(new ImproveSettings {WordCount = 2, NoiseLength = 3})
    // scrape result pages and extract distinct words
    .With<Wordify>(new WordifySettings {IncludeTitle = true, NoiseLength = 3});
// execute services (order of addition)
var results = await context.SearchAsync("quantum physics");
// do anything you need with LINQ
var scientific = results.Where(result => result.Url.Contains("wikipedia"));

foreach (var result in scientific) {
    Console.WriteLine($"{result.Url} ${result.Description}");
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

## Icon

[Search Engine](https://thenounproject.com/search/?q=search%20engine&i=2054907) icon designed by Vectors Market from [The Noun Project](https://thenounproject.com/).

### Notes

- PickAll is still under development and API could change until latest beta versions.