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
$ ./artifacts/PickAll.Simple/Debug/netcoreapp3.0/PickAll.Simple
[5] DUCKDUCKGO: "Brief History of Steve Jobs and Apple": "https://www.thebalancecareers.com/industry-spotlight-steve-jobs-38936"
[10] GOOGLE: "Apple Inc.": "https://en.wikipedia.org/wiki/Apple_Inc."
[12] DUCKDUCKGO: "Steve Jobs and the Apple Story - Investopedia": "https://www.investopedia.com/articles/fundamental-analysis/12/steve-jobs-apple-story.asp"
[13] GOOGLE: "In ricordo di Steve Jobs - Apple (IT)": "https://www.apple.com/it/stevejobs/"
[14] GOOGLE: "Remembering Steve Jobs - Apple": "https://www.apple.com/stevejobs/"
[17] GOOGLE: "Steve Jobs - Apple, Family & Death - Biography": "https://www.biography.com/business-figure/steve-jobs"
[19] DUCKDUCKGO: "Steve Jobs | Biography, Apple, & Facts | Britannica": "https://www.britannica.com/biography/Steve-Jobs
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
using PickAll.Searchers;
using PickAll.PostProcessors;

var context = new SearchContext()
    .With<Google>() // search on google.com
    .With<DuckDuckGo>() // search on duckduckgo.com
    .With<Uniqueness>() // remove duplicates
    .With<Order>(); // order results by index
// execute services (order of addition)
var results = await context.Search("quantum physics");
// do anything you need with LINQ
var scientific = results.Where(result => result.Url.Contains("scien"));

foreach (var result in scientific) {
    Console.WriteLine($"{result.Url} ${result.Description}");
}
```

### Notes
- This is a pre-release, since it's under development API can change until stable version.