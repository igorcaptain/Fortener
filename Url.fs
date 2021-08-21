namespace Fortener

type Url = {
    Id : string
    ShortRoute : string
    OriginalUrl : string
}

type UrlWe = {
    Url: string
}

type UrlGenerate = string -> Url option

type UrlSave = Url -> Url

type UrlCriteria = 
    | All
    | ByShortRoute of shortRoute : string

type UrlFind = UrlCriteria -> Url[]