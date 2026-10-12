namespace Belin.FreeMobile

open System
open System.Net
open System.Net.Http
open System.Threading
open System.Web

/// Sends messages by SMS to a [FreeMobile](https://mobile.free.fr) account.
type Client(credential: NetworkCredential) =

  /// The assembly version.
  static let version = nonNull (typeof<Client>.Assembly.GetName().Version)

  /// Value indicating whether this object has been disposed.
  let mutable disposed = false

  /// The underlying HTTP client.
  let httpClient = new HttpClient(Timeout = TimeSpan.FromMinutes 1L)

  /// The base URL of the remote API endpoint.
  member val BaseUrl = Uri "https://smsapi.free-mobile.fr/" with get, set

  /// The Free Mobile user name and password.
  member val Credential: NetworkCredential = credential with get, set

  /// The user agent string to use when making requests.
  member val UserAgent = $".NET/{Environment.Version} | Belin.FreeMobile.FSharp/{version.ToString 3}" with get, set

  /// Creates a new client.
  new(userName: string, password: string) = new Client(NetworkCredential(userName, password))

  interface IDisposable with
    /// Releases any resources associated with this object.
    member this.Dispose() =
      this.Dispose(disposing = true)
      GC.SuppressFinalize this

  /// Sends an SMS message to the underlying account.
  member this.SendMessageAsync(text: string, ?cancellationToken: CancellationToken): Async<Result<unit, HttpRequestException>> = async {
    let trimmedText = text.Trim()
    let queryString = HttpUtility.ParseQueryString ""
    queryString.Add("msg", if trimmedText.Length > 160 then trimmedText[0..159] else trimmedText)
    queryString.Add("pass", this.Credential.Password)
    queryString.Add("user", this.Credential.UserName)

    use request = new HttpRequestMessage(HttpMethod.Get, Uri(this.BaseUrl, $"sendmsg?{queryString}"))
    request.Headers.Add("User-Agent", this.UserAgent)

    try
      let! ambientToken = Async.CancellationToken
      use! response = httpClient.SendAsync(request, defaultArg cancellationToken ambientToken) |> Async.AwaitTask
      response.EnsureSuccessStatusCode() |> ignore<HttpResponseMessage>
      return Ok ()
    with
    | :? HttpRequestException as ex -> return Error ex
    | :? AggregateException as ex ->
      match ex.InnerException with
      | :? HttpRequestException as innerEx -> return Error innerEx
      | _ -> return raise (ex.Flatten())
  }

  /// Sends an SMS message to the underlying account.
  member this.SendMessage(text: string, ?cancellationToken: CancellationToken): Result<unit, HttpRequestException> =
    this.SendMessageAsync(text, ?cancellationToken = cancellationToken) |> Async.RunSynchronously // fsharplint:disable-line TupleCommaSpacing

  /// Releases any resources associated with this object.
  member private _.Dispose(disposing: bool) =
    if not disposed then
      if disposing then httpClient.Dispose()
      disposed <- true
