#r "nuget: Belin.FreeMobile.FSharp"
open Belin.FreeMobile

// Sends an SMS notification.
async {
  use client = new Client("your account identifier", "your API key")
  match! client.AsyncSendMessage "Hello World from F#" with
  | Ok _ -> printn "The message was sent successfully."
  | Error error -> printn $"An error occurred: {error.Message}"
}
