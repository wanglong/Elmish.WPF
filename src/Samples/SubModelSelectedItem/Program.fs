﻿module Elmish.WPF.Samples.SubModelSelectedItem.Program

open System
open Elmish
open Elmish.WPF

type Entity =
  { Id: int
    Name: string }

type Model =
  { Entities: Entity list
    Selected: int option }

let init () =
  { Entities = [0 .. 10] |> List.map (fun i -> { Id = i; Name = sprintf "Entity %i" i})
    Selected = Some 4 }

type Msg =
  | Select of int option

let update msg m =
  match msg with
  | Select entityId -> { m with Selected = entityId }

let bindings () = [
  "SelectRandom" |> Binding.cmd
    (fun m -> m.Entities.Item(Random().Next(m.Entities.Length)).Id |> Some |> Select)
  "Deselect" |> Binding.cmd (fun _ -> Select None)
  "Entities" |> Binding.subModelSeq
    (fun m -> m.Entities)
    id
    (fun (m, e) -> e.Id)
    snd
    (fun () -> [
      "Name" |> Binding.oneWay (fun (_, e) -> e.Name)
      "SelectedLabel" |> Binding.oneWay (fun (m, e) -> if m.Selected = Some e.Id then " - SELECTED" else "")
    ])
  "SelectedEntity" |> Binding.subModelSelectedItem
    "Entities"
    (fun m -> m.Selected)
    (fun id m -> Select id)
]


[<EntryPoint; STAThread>]
let main argv =
  Program.mkSimple init update (fun _ _ -> bindings ())
  |> Program.withConsoleTrace
  |> Program.runWindowWithConfig
      { ElmConfig.Default with LogConsole = true }
      (MainWindow())
