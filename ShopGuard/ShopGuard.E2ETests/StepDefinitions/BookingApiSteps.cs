using System.Net.Http.Json;
using AwesomeAssertions;
using Reqnroll;
using ShopGuard.Core.Helpers;
using ShopGuard.Core.Models;
using ShopGuard.E2ETests.Support;

namespace ShopGuard.E2ETests.StepDefinitions;

[Binding]
public sealed class BookingApiSteps(ApiContext api, ScenarioState state)
{
    [When("ich den API-Healthcheck aufrufe")]
    public async Task WhenCallHealthcheck()
        => state.LastStatusCode = await api.Client.PingAsync() ? 201 : 0;

    [Then("ist die API erreichbar")]
    public void ThenApiIsReachable()
        => state.LastStatusCode.Should().Be(201, "GET /ping muss laut Contract 201 liefern");

    [When("ich ein Auth-Token mit gültigen Zugangsdaten anfordere")]
    public async Task WhenRequestTokenWithValidCredentials()
        => state.AuthToken = await api.Client.AuthenticateAsync(
            TestConfiguration.Settings.ApiUsername, TestConfiguration.Settings.ApiPassword);

    [When("ich ein Auth-Token mit dem Benutzer {string} und dem Passwort {string} anfordere")]
    public async Task WhenRequestTokenWithCredentials(string username, string password)
        => state.AuthToken = await api.Client.AuthenticateAsync(username, password);

    [Then("erhalte ich ein gültiges Token")]
    public void ThenTokenIsValid()
        => state.AuthToken.Should().NotBeNullOrWhiteSpace();

    [Then("erhalte ich kein Token")]
    public void ThenNoToken()
        => state.AuthToken.Should().BeNull();

    [Given("ich habe eine gültige Buchung für {string} vorbereitet")]
    public void GivenValidBookingPrepared(string firstName)
        => state.RequestBooking = TestDataGenerator.DefaultBooking(firstName);

    [Given("ich habe eine Buchung mit Vorname {string}, Preis {int} und Anzahlung {string} vorbereitet")]
    public void GivenBookingWithData(string firstName, int totalPrice, string depositPaid)
    {
        var template = TestDataGenerator.DefaultBooking(firstName);
        state.RequestBooking = new Booking
        {
            FirstName = firstName,
            LastName = template.LastName,
            TotalPrice = totalPrice,
            DepositPaid = bool.Parse(depositPaid),
            BookingDates = template.BookingDates,
            AdditionalNeeds = template.AdditionalNeeds,
        };
    }

    [Given("ich die Buchung über die API angelegt habe")]
    [When("ich die Buchung über die API anlege")]
    public async Task WhenCreateBooking()
        => state.CreatedBooking = await api.Client.CreateBookingAsync(state.RequestBooking!);

    [Given("ich als API-Administrator authentifiziert bin")]
    public async Task GivenAuthenticatedAsAdmin()
    {
        var token = await api.Client.AuthenticateAsync(
            TestConfiguration.Settings.ApiUsername, TestConfiguration.Settings.ApiPassword);
        token.Should().NotBeNullOrWhiteSpace("ohne Token sind keine Schreiboperationen möglich");
    }

    [Then("erhalte ich eine Buchungsnummer")]
    public void ThenBookingIdReturned()
        => state.CreatedBooking!.BookingId.Should().BePositive();

    [Then("entsprechen die zurückgegebenen Buchungsdaten der Anfrage")]
    public void ThenEchoedBookingMatchesRequest()
        => state.CreatedBooking!.Booking.Should().BeEquivalentTo(state.RequestBooking);

    [When("ich die Buchung über ihre Id abrufe")]
    public async Task WhenFetchCreatedBooking()
        => state.FetchedBooking = await api.Client.GetBookingAsync(state.CreatedBooking!.BookingId);

    [When("ich die Buchung mit der Id {int} abrufe")]
    public async Task WhenFetchBookingById(int bookingId)
        => state.FetchedBooking = await api.Client.GetBookingAsync(bookingId);

    [Then("entsprechen die abgerufenen Buchungsdaten der Anfrage")]
    public void ThenFetchedBookingMatchesRequest()
        => state.FetchedBooking.Should().BeEquivalentTo(state.RequestBooking);

    [Then("hat die angelegte Buchung den Preis {int} und Anzahlung {string}")]
    public void ThenCreatedBookingHasPriceAndDeposit(int totalPrice, string depositPaid)
    {
        state.CreatedBooking!.Booking.TotalPrice.Should().Be(totalPrice);
        state.CreatedBooking.Booking.DepositPaid.Should().Be(bool.Parse(depositPaid));
    }

    [When("ich den Nachnamen der Buchung auf {string} ändere")]
    public async Task WhenChangeLastName(string newLastName)
    {
        var updated = new Booking
        {
            FirstName = state.RequestBooking!.FirstName,
            LastName = newLastName,
            TotalPrice = state.RequestBooking.TotalPrice,
            DepositPaid = state.RequestBooking.DepositPaid,
            BookingDates = state.RequestBooking.BookingDates,
            AdditionalNeeds = state.RequestBooking.AdditionalNeeds,
        };
        await api.Client.UpdateBookingAsync(state.CreatedBooking!.BookingId, updated);
    }

    [Then("hat die Buchung beim erneuten Abruf den Nachnamen {string}")]
    public async Task ThenBookingHasLastName(string expectedLastName)
    {
        var booking = await api.Client.GetBookingAsync(state.CreatedBooking!.BookingId);
        booking!.LastName.Should().Be(expectedLastName);
    }

    [When("ich die Buchung über die API lösche")]
    public Task WhenDeleteBooking()
        => api.Client.DeleteBookingAsync(state.CreatedBooking!.BookingId);

    [Then("ist die Buchung nicht mehr abrufbar")]
    public async Task ThenBookingIsGone()
        => (await api.Client.GetBookingAsync(state.CreatedBooking!.BookingId)).Should().BeNull();

    [Then("ist die Buchung nicht vorhanden")]
    public void ThenBookingNotFound()
        => state.FetchedBooking.Should().BeNull();

    [When("ich die Buchung ohne Token zu aktualisieren versuche")]
    public async Task WhenUpdateWithoutToken()
    {
        // Bypasses the typed method on purpose to probe the raw status code.
        using var request = new HttpRequestMessage(
            HttpMethod.Put, $"booking/{state.CreatedBooking!.BookingId}")
        {
            Content = JsonContent.Create(state.RequestBooking),
        };
        var response = await api.Client.SendRawAsync(request);
        state.LastStatusCode = (int)response.StatusCode;
    }

    [When("ich eine Buchung mit unvollständiger Payload anlege")]
    public async Task WhenCreateBookingWithIncompletePayload()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "booking")
        {
            // Mandatory fields (lastname, totalprice, bookingdates) are missing on purpose.
            Content = JsonContent.Create(new { firstname = "OnlyFirstName" }),
        };
        var response = await api.Client.SendRawAsync(request);
        state.LastStatusCode = (int)response.StatusCode;
    }

    [Then("wird die Anfrage mit Status {int} abgelehnt")]
    public void ThenRequestRejectedWithStatus(int expectedStatusCode)
        => state.LastStatusCode.Should().Be(expectedStatusCode);

    [Then("erfüllt die Antwort den Buchungs-Contract")]
    public void ThenResponseFulfillsContract()
    {
        // Successful strongly-typed deserialization already proves field names and types;
        // here we additionally pin the mandatory values.
        var booking = state.FetchedBooking!;
        booking.FirstName.Should().NotBeNullOrWhiteSpace();
        booking.LastName.Should().NotBeNullOrWhiteSpace();
        booking.TotalPrice.Should().BeGreaterThanOrEqualTo(0);
        booking.BookingDates.Should().NotBeNull();
        booking.BookingDates.CheckOut.Should().BeAfter(booking.BookingDates.CheckIn);
    }
}
