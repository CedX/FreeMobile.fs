#r "nuget: Belin.FreeMobile.FSharp"
open Belin.FreeMobile

// Sends an SMS notification.
async {
  use client = new Client("your account identifier", "your API key")
  match! client.SendMessageAsync "Hello World from F#" with
  | Ok _ -> printfn "The message was sent successfully."
  | Error error -> printfn $"An error occurred: {error.Message}"
}
