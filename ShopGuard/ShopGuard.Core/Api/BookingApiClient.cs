using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ShopGuard.Core.Models;

namespace ShopGuard.Core.Api;

/// <summary>
/// Typed client for the restful-booker API (https://restful-booker.herokuapp.com).
/// The HttpClient is injected so tests can mock the message handler;
/// its BaseAddress must point to the API root.
/// </summary>
public sealed class BookingApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient httpClient;

    public BookingApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        // restful-booker answers 418 "I'm a Teapot" when no Accept header is sent.
        if (!httpClient.DefaultRequestHeaders.Accept.Any())
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }

    /// <summary>Auth token used as cookie for write operations (PUT/PATCH/DELETE).</summary>
    public string? Token { get; private set; }

    /// <summary>
    /// Requests a token via POST /auth. The API answers HTTP 200 even for bad
    /// credentials and signals failure through a "reason" field instead.
    /// </summary>
    /// <returns>The token, or null when the credentials were rejected.</returns>
    public async Task<string?> AuthenticateAsync(string username, string password, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync(
            "auth", new AuthRequest { Username = username, Password = password }, JsonOptions, ct);
        await EnsureSuccessAsync(response, ct);

        var auth = await ReadAsAsync<AuthResponse>(response, ct);
        Token = auth.Token;
        return auth.Token;
    }

    /// <summary>Checks API availability via GET /ping (returns 201 by contract).</summary>
    public async Task<bool> PingAsync(CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync("ping", ct);
        return response.StatusCode == HttpStatusCode.Created;
    }

    /// <summary>Returns all booking ids via GET /booking.</summary>
    public async Task<IReadOnlyList<int>> GetBookingIdsAsync(CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync("booking", ct);
        await EnsureSuccessAsync(response, ct);

        var ids = await ReadAsAsync<List<BookingId>>(response, ct);
        return ids.Select(b => b.Id).ToList();
    }

    /// <summary>Returns a single booking via GET /booking/{id}, or null when it does not exist.</summary>
    public async Task<Booking?> GetBookingAsync(int bookingId, CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"booking/{bookingId}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response, ct);
        return await ReadAsAsync<Booking>(response, ct);
    }

    /// <summary>Creates a booking via POST /booking.</summary>
    public async Task<BookingCreatedResponse> CreateBookingAsync(Booking booking, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync("booking", booking, JsonOptions, ct);
        await EnsureSuccessAsync(response, ct);
        return await ReadAsAsync<BookingCreatedResponse>(response, ct);
    }

    /// <summary>Replaces a booking via PUT /booking/{id}. Requires a prior <see cref="AuthenticateAsync"/>.</summary>
    public async Task<Booking> UpdateBookingAsync(int bookingId, Booking booking, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"booking/{bookingId}")
        {
            Content = JsonContent.Create(booking, options: JsonOptions),
        };
        AddAuthCookie(request);

        var response = await httpClient.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);
        return await ReadAsAsync<Booking>(response, ct);
    }

    /// <summary>Deletes a booking via DELETE /booking/{id}. Requires a prior <see cref="AuthenticateAsync"/>.</summary>
    public async Task DeleteBookingAsync(int bookingId, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"booking/{bookingId}");
        AddAuthCookie(request);

        var response = await httpClient.SendAsync(request, ct);
        // The API answers 201 Created on successful delete — another documented quirk.
        if (response.StatusCode is not (HttpStatusCode.Created or HttpStatusCode.OK))
        {
            throw await ToApiExceptionAsync(response, ct);
        }
    }

    /// <summary>Sends a raw status-code probe; used by negative tests that bypass the typed methods.</summary>
    public Task<HttpResponseMessage> SendRawAsync(HttpRequestMessage request, CancellationToken ct = default)
        => httpClient.SendAsync(request, ct);

    private void AddAuthCookie(HttpRequestMessage request)
    {
        if (Token is null)
        {
            throw new InvalidOperationException(
                "No auth token available. Call AuthenticateAsync before write operations.");
        }

        request.Headers.Add("Cookie", $"token={Token}");
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw await ToApiExceptionAsync(response, ct);
        }
    }

    private static async Task<ApiException> ToApiExceptionAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var body = await response.Content.ReadAsStringAsync(ct);
        return new ApiException(
            response.StatusCode,
            body,
            $"{response.RequestMessage?.Method} {response.RequestMessage?.RequestUri} failed with {(int)response.StatusCode} {response.StatusCode}.");
    }

    private static async Task<T> ReadAsAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct);
        return result ?? throw new ApiException(
            response.StatusCode, null, "Response body was empty or could not be deserialized.");
    }
}
