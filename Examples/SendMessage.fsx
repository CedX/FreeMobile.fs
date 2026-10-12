#r "nuget: Belin.FreeMobile.FSharp"
open Belin.FreeMobile

// Sends an SMS notification.
do
  use client = new Client("your account identifier", "your API key")
  match client.SendMessageAsync "Hello World from F#" |> Async.RunSynchronously with
  | Ok _ -> printfn "The message was sent successfully."
  | Error error -> printfn $"An error occurred: {error.Message}"
