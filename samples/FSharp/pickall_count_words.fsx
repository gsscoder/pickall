(*
* pickall_count_words.fsx
* - F# Script that demonstrates the use of PickAll (github.com/gsscoder/pickall)
* - Searches the web and counts the words of results descriptions
* - Requires PickAll.dll and AngleSharp.dll in the script directory
*)
#r "PickAll.dll"
open System
open Microsoft.FSharp.Control
open PickAll
open PickAll.Searchers

let alpha (s : string) =
   not (s.ToCharArray() 
   |> Seq.map (fun x -> Char.IsLetterOrDigit(x) && not (Char.IsWhiteSpace(x)))
   |> Seq.contains false)

let query = "Steve Jobs"

let context = new SearchContext(typeof<Google>,
                                typeof<DuckDuckGo>,
                                typeof<Yahoo>)
let results = context.SearchAsync(query)
              |> Async.AwaitTask
              |> Async.RunSynchronously
let words = results
           |> Seq.map (fun x -> x.Description.Split())
           |> Seq.concat
words |> Seq.filter alpha 
      |> Seq.countBy id
      |> Seq.sortBy (fun (_, y) -> -y)
      |> Seq.iter (fun (x, y) -> printfn "%s %d" x y)