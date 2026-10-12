namespace Belin.FreeMobile

open Microsoft.VisualStudio.TestTools.UnitTesting
open System

/// Tests the features of the `Client` class.
[<TestClass>]
type ClientTests(testContext: TestContext) =

  [<TestMethod>]
  member _.NetworkError() =
    use client = new Client("anonymous", "secret", BaseUrl = Uri "http://localhost:666")

    // It should throw a `HttpRequestException` if a network error occurred.
    client.SendMessageAsync("Hello World!", testContext.CancellationToken)
    |> Async.RunSynchronously
    |> Result.isError
    |> Should.beTrue

  [<TestMethod>]
  member _.InvalidCredentials() =
    use client = new Client("anonymous", "secret")

    // It should throw a `HttpRequestException` if the credentials are invalid.
    client.SendMessageAsync("Hello World!", testContext.CancellationToken)
    |> Async.RunSynchronously
    |> Result.isError
    |> Should.beTrue

  [<TestMethod>]
  member _.ValidCredentials() =
    let userName = Environment.GetEnvironmentVariable "FREEMOBILE_ACCOUNT"
    let password = Environment.GetEnvironmentVariable "FREEMOBILE_API_KEY"
    use client = new Client(nonNull userName, nonNull password)

    // It should send SMS messages if the credentials are valid.
    client.SendMessageAsync("Hello Cédric, from F#", testContext.CancellationToken)
    |> Async.RunSynchronously
    |> Result.isError
    |> Should.beFalse
