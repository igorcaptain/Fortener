open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Fortener.Http
open Url.UrlInMemory
open System.Collections

let routes =
    choose [
        UrlHttp.handlers
    ]

let configureApp (app : IApplicationBuilder) =
    app.UseGiraffe routes

let configureServices (services : IServiceCollection) =
    services.AddGiraffe() |> ignore
    services.AddUrlInMemory(Hashtable()) |> ignore

[<EntryPoint>]
let main argv =
    WebHostBuilder()
        .UseKestrel()
        .Configure(configureApp)
        .ConfigureServices(configureServices)
        .Build()
        .Run()
    0