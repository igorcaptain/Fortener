open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Fortener.Http
open MongoDB.Driver
open System
open Url.UrlMongoDb
open Fortener

let routes =
    choose [
        UrlHttp.handlers
    ]

let configureApp (app : IApplicationBuilder) =
    app.UseGiraffe routes

let configureServices (services : IServiceCollection) =
    let mongo = MongoClient(Environment.GetEnvironmentVariable "MONGO_URL")
    let db = mongo.GetDatabase "fortener"
    services.AddGiraffe() |> ignore
    services.AddUrlMongoDb(db.GetCollection<Url>("urls")) |> ignore

[<EntryPoint>]
let main argv =
    WebHostBuilder()
        .UseKestrel()
        .Configure(configureApp)
        .ConfigureServices(configureServices)
        .Build()
        .Run()
    0