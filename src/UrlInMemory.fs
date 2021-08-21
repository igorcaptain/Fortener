module Url.UrlInMemory

open Fortener
open Microsoft.Extensions.DependencyInjection
open System.Collections
open System
open Giraffe

let find (inMemory: Hashtable) (criteria: UrlCriteria): Url[] =
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

let isValidUrl (url: string) =

    //     | Uri.UriSchemeHttp 
    //     | Uri.UriSchemeHttps 
    //     | Uri.UriSchemeFtp 
    //     | Uri.UriSchemeMailto -> true

    // version 2
    match Uri.TryCreate(url, UriKind.Absolute) with
    | true, uriResult -> true
    | _ -> false

let createUrl (originalUrl: string) =
    let shortGuid = ShortGuid.fromGuid(Guid.NewGuid())
    { 
        Id = shortGuid; 
        ShortRoute = shortGuid.[0..5]; 
        OriginalUrl = originalUrl
    }

let getUrlWithMethod (originalUrl: string) =
    "https://" + originalUrl

let generate (originalUrl: string) =
    match isValidUrl originalUrl with
    | true -> Some(createUrl originalUrl)
    | false -> 
        let tryUrl = getUrlWithMethod originalUrl
        match isValidUrl tryUrl with
        | true -> Some(createUrl tryUrl)
        | _ -> None

let save (inMemory: Hashtable) (url: Url) : Url =
    inMemory.Add(url.Id, url) |> ignore
    url

type IServiceCollection with
    member this.AddUrlInMemory (inMemory: Hashtable) =
        this.AddSingleton<UrlFind>(find inMemory) |> ignore
        this.AddSingleton<UrlSave>(save inMemory) |> ignore
        this.AddSingleton<UrlGenerate>(generate) |> ignore