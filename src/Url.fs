namespace Fortener
open System
open Giraffe

type Url = {
    Id : string
    ShortRoute : string
    OriginalUrl : string
}

type UrlWe = {
    Url : string
}

type UrlGenerate = string -> Url option

type UrlSave = Url -> Url

type UrlDelete = string -> bool

type UrlCriteria = 
    | All
    | ByShortRoute of shortRoute : string

type UrlFind = UrlCriteria -> Url[]

module Url = 
    
    let isValidUrl (url : string) =
        match Uri.TryCreate(url, UriKind.Absolute) with
        | true, uriResult -> true
        | _ -> false

    let createUrl (originalUrl : string) =
        let shortGuid = ShortGuid.fromGuid(Guid.NewGuid())
        { 
            Id = shortGuid; 
            ShortRoute = shortGuid.[0..5]; 
            OriginalUrl = originalUrl
        }

    let getUrlWithMethod (originalUrl : string) =
        "https://" + originalUrl

    let generate (originalUrl : string) =
        match isValidUrl originalUrl with
        | true -> Some(createUrl originalUrl)
        | false -> 
            let tryUrl = getUrlWithMethod originalUrl
            match isValidUrl tryUrl with
            | true -> Some(createUrl tryUrl)
            | _ -> None
