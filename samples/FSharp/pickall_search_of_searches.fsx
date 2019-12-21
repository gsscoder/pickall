(*
* pickall_search_of_searches.fsx
* - F# Script that demonstrates the use of PickAll (github.com/gsscoder/pickall)
* - Searches the web adding the first two more used words in initial results
* - descriptions for a subsequent query
* - Requires PickAll.dll (0.10.0) and AngleSharp.dll (0.14.0-alpha-787)
*   in the script directory
*)
#r "PickAll.dll"
open System
open Microsoft.FSharp.Control
open PickAll
open PickAll.Searchers
open PickAll.PostProcessors

let alpha (s : string) =
   not (s.ToCharArray() 
   |> Seq.map (fun x -> Char.IsLetterOrDigit(x) && not (Char.IsWhiteSpace(x)))
   |> Seq.contains false)

let query = "Bill Gates"

let context = new SearchContext(typeof<Google>,
                                typeof<DuckDuckGo>,
                                typeof<Yahoo>)
let results = context.SearchAsync(query)
              |> Async.AwaitTask
              |> Async.RunSynchronously
let words = results
            |> Seq.map (fun x -> x.Description.Split())
            |> Seq.concat
let exclude = query.Split()
let newQuery = (words |> Seq.filter alpha
            |> Seq.filter (fun x -> exclude |> Seq.contains x |> not)
            |> Seq.countBy id
            |> Seq.sortBy (fun (_, y) -> -y)
            |> Seq.take 2
            |> Seq.map (fun (x, _) -> x + " ")
            |> String.Concat).Trim() + " " + query
let newContext = context
                  .With<Uniqueness>()
                  .With<Order>()
newContext.SearchAsync(newQuery)
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> Seq.iter (fun x -> printfn "%s - %s" (x.Description.ToUpper()) x.Url)