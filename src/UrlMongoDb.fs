module Url.UrlMongoDb

open Fortener
open Microsoft.Extensions.DependencyInjection
open MongoDB.Driver

let find (collection : IMongoCollection<Url>) (criteria : UrlCriteria): Url[] =
    match criteria with
    | All ->
        collection.Find(Builders.Filter.Empty).ToEnumerable() 
        |> Seq.toArray
    | ByShortRoute route ->
        collection.Find(Builders.Filter.Eq((fun url -> url.ShortRoute), route)).ToEnumerable() 
        |> Seq.toArray

let save (collection : IMongoCollection<Url>) (url : Url) : Url =
    let urls = collection.Find(fun x -> x.OriginalUrl = url.OriginalUrl).ToEnumerable()

    match Seq.isEmpty urls with
    | true -> 
        collection.InsertOne url
        url
    | false -> 
        let filter = Builders<Url>.Filter.Eq((fun x -> x.OriginalUrl), url.OriginalUrl)
        let existingUrl = collection.Find(filter).Single()
        existingUrl

let delete (collection : IMongoCollection<Url>) (id : string) : bool =
    collection.DeleteOne(Builders<Url>.Filter.Eq((fun x -> x.Id), id)).DeletedCount > 0L


type IServiceCollection with
    member this.AddUrlMongoDb (collection : IMongoCollection<Url>) =
        this.AddSingleton<UrlFind>(find collection) |> ignore
        this.AddSingleton<UrlSave>(save collection) |> ignore
        this.AddSingleton<UrlDelete>(delete collection) |> ignore