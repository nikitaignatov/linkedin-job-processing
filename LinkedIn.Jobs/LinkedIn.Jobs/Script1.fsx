﻿#load "Loader.fs"
#r "FSharp.Core"
#r @"..\packages\FSharp.Data.2.3.1\lib\net40\FSharp.Data.dll"

open FSharp.Data
open LinkedIn
open LinkedIn.Jobs
open LinkedIn.Jobs.Loader
open System.Text.RegularExpressions

type Job = JsonProvider< "samples/job.json" >

type Jobs = JsonProvider< "samples/jobs.json" >

let root = @"c:\temp\linkedin-jobs\"
let path file = System.IO.Path.Combine(root, file)
let [| dump; collected; normalized; csv |] = [| "dump.json"; "collected.json"; "normalized.json"; "final.csv" |] |> Array.map path

Loader.create dir
|> extract country
|> Array.collect (extract date)
|> Array.collect (extract keyword)
|> Array.collect (files file)
|> Array.filter isJobFile
|> Array.sortBy byDate
|> Array.rev
|> Array.distinctBy byJobId
|> Array.map openFile
|> join
|> write dump

let jobs = 
    Loader.create dump
    |> openFile
    |> fun c -> Jobs.Parse c.data

let summary (f : 'a -> int option) = 
    jobs
    |> Array.map f
    |> Array.choose id
    |> Array.sum

let sort (f : 'a -> 'b) = jobs |> Array.sortBy f
let sortDesc (f : 'a -> 'b) = jobs |> Array.sortByDescending f

// top 10 jobs with most applications
let top = 
    sortDesc (fun c -> c.Applicants)
    |> Array.take 10
    |> Array.map (fun c -> c.Company.Name, c.Day, c.Views, c.Applicants)

summary (fun c -> c.Views)
//summary (fun c -> int c.Applicants)
jobs
|> Array.distinctBy (fun c -> c.Id)
|> Array.length

