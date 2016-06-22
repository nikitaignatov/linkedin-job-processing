namespace LinkedIn.Jobs

(* Pretend the files are stored in the following structure:

+---dk
|   +---2016-05-31
|   |   +---java
|   |   +---software
|   |   +---tester
|   |   \---udvikler
|   +---2016-06-01
|       +---java
|       +---software
|       \---udvikler
\---ma
    +---2016-05-31
    |   +---java
    |   +---software
    |   \---tester
    +---2016-06-01
        +---java
        +---software
        \---tester
*)
module Loader = 
    open System
    open System.IO
    open System.Text.RegularExpressions
    
    type T = 
        { path : string
          country : string
          date : string
          keyword : string
          file : string
          data : string }
    
    let create path = 
        { path = path
          country = ""
          date = ""
          keyword = ""
          file = ""
          data = "" }
    
    let dir = @"c:\temp\linkedin-jobs\dump\"
    let get dir = Directory.GetDirectories dir
    let someNames (dir : string) = dir.Split '\\' |> Array.tryLast
    
    let extract (map : T -> string -> T) (state : T) = 
        get state.path
        |> Array.map someNames
        |> Array.choose id
        |> Array.map (map state)
    
    let cd dir next = Path.Combine(dir, next)
    let files map (s : T) = Directory.GetFiles s.path |> Array.map (map s)
    let openFile (s : T) = { s with data = File.ReadAllText s.path }
    let write path data = File.WriteAllText(path, data)
    let join (ary : T array) = sprintf "[%s]" (System.String.Join(",\n", ary |> Array.map (fun c -> c.data)))
    let isJobFile (s : T) = Regex.IsMatch(s.file, "^job_\d+.json$")
    let byJobId (s : T) = Regex.Replace(s.file, "^job_(\d+).json$", "$1")
    let byDate (s : T) = Regex.Replace(s.date, @".*(\d+-\d+-\d+)\.*$", "$1")
