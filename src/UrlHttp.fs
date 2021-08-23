namespace Fortener.Http

open Giraffe
open Microsoft.AspNetCore.Http
open Fortener
open FSharp.Control.Tasks

module UrlHttp =
    let handlers : HttpFunc -> HttpContext -> HttpFuncResult =
        choose [
            POST >=> route "/url" >=>
                fun next context ->
                    task {
                        let save = context.GetService<UrlSave>()
                        let! urlWe = context.BindJsonAsync<UrlWe>()
                        let url = Url.generate urlWe.Url
                        match url with
                        | Some value -> return! json (save value) next context
                        | None ->
                            let response = {| message = "Url format invalid" |}
                            return! (setStatusCode 400 >=> json response) next context
                    }

            POST >=> route "/urls" >=>
                fun next context ->
                    task {
                        let save = context.GetService<UrlSave>()
                        let! urlWes = context.BindJsonAsync<UrlWe[]>()
                        let result = urlWes |> Array.map (fun urlWe ->
                            let url = Url.generate urlWe.Url
                            match url with
                            | Some value -> Some (save value)
                            | None -> None) |> Array.choose id
                        return! json result next context
                    }

            GET >=> route "/urls" >=>
                fun next context ->
                    let find = context.GetService<UrlFind>()
                    let urls = find UrlCriteria.All
                    json urls next context

            GET >=> routef "/%s" (fun route ->
                fun next context ->
                    let find = context.GetService<UrlFind>()
                    let urls = find (UrlCriteria.ByShortRoute route)
                    match Array.length urls with
                    | 1 -> redirectTo false urls.[0].OriginalUrl next context
                    | _ -> redirectTo false "/" next context
            )

            DELETE >=> routef "/url/%s" (fun id ->
                fun next context ->
                    let delete = context.GetService<UrlDelete>()
                    json (delete id) next context
            )

            RequestErrors.NOT_FOUND "Not Found" 
        ]