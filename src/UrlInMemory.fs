module Url.UrlInMemory

open Fortener
open Microsoft.Extensions.DependencyInjection
open System.Collections

let find (inMemory : Hashtable) (criteria : UrlCriteria) : Url[] =
    match criteria with
    | All ->
        inMemory.Values 
        |> Seq.cast 
        |> Array.ofSeq
    | ByShortRoute route ->
        inMemory.Values 
        |> Seq.cast 
        |> Seq.filter (fun x -> x.ShortRoute = route)
        |> Array.ofSeq

let save (inMemory : Hashtable) (url : Url) : Url =
    inMemory.Add(url.Id, url) |> ignore
    url

type IServiceCollection with
    member this.AddUrlInMemory (inMemory : Hashtable) =
        this.AddSingleton<UrlFind>(find inMemory) |> ignore
        this.AddSingleton<UrlSave>(save inMemory) |> ignore