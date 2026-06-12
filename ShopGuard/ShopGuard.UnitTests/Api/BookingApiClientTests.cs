using System.Net;
using AwesomeAssertions;
using ShopGuard.Core.Api;
using ShopGuard.Core.Helpers;
using ShopGuard.UnitTests.Support;

namespace ShopGuard.UnitTests.Api;

[TestFixture]
public sealed class BookingApiClientTests
{
    [Test]
    public async Task AuthenticateAsync_PostsCredentialsToAuthEndpoint()
    {
        var factory = new HttpMockFactory().Respond(HttpStatusCode.OK, """{"token":"abc123"}""");
        var client = factory.CreateClient();

        await client.AuthenticateAsync("admin", "password123");

        var request = factory.Requests.Single();
        request.Method.Should().Be(HttpMethod.Post);
        request.Uri.Should().Be($"{HttpMockFactory.BaseAddress}auth");
        request.Body.Should().Contain("\"username\":\"admin\"").And.Contain("\"password\":\"password123\"");
    }

    [Test]
    public async Task AuthenticateAsync_OnSuccess_ReturnsAndStoresToken()
    {
        var client = new HttpMockFactory()
            .Respond(HttpStatusCode.OK, """{"token":"abc123"}""")
            .CreateClient();

        var token = await client.AuthenticateAsync("admin", "password123");

        token.Should().Be("abc123");
        client.Token.Should().Be("abc123");
    }

    [Test]
    public async Task AuthenticateAsync_OnBadCredentials_ReturnsNull()
    {
        // The API signals bad credentials with HTTP 200 and a "reason" field.
        var client = new HttpMockFactory()
            .Respond(HttpStatusCode.OK, """{"reason":"Bad credentials"}""")
            .CreateClient();

        var token = await client.AuthenticateAsync("admin", "wrong");

        token.Should().BeNull();
        client.Token.Should().BeNull();
    }

    [Test]
    public async Task GetBookingAsync_RequestsCorrectUrl_AndDeserializesBooking()
    {
        var factory = new HttpMockFactory().Respond(HttpStatusCode.OK, """
            {
              "firstname": "Jim", "lastname": "Brown", "totalprice": 111, "depositpaid": true,
              "bookingdates": { "checkin": "2026-01-01", "checkout": "2026-01-05" },
              "additionalneeds": "Breakfast"
            }
            """);
        var client = factory.CreateClient();

        var booking = await client.GetBookingAsync(42);

        factory.Requests.Single().Uri.Should().Be($"{HttpMockFactory.BaseAddress}booking/42");
        booking.Should().NotBeNull();
        booking.FirstName.Should().Be("Jim");
        booking.TotalPrice.Should().Be(111);
        booking.BookingDates.CheckOut.Should().Be(new DateOnly(2026, 1, 5));
    }

    [Test]
    public async Task GetBookingAsync_On404_ReturnsNull()
    {
        var client = new HttpMockFactory().Respond(HttpStatusCode.NotFound).CreateClient();

        var booking = await client.GetBookingAsync(99999);

        booking.Should().BeNull();
    }

    [Test]
    public async Task GetBookingIdsAsync_ReturnsAllIds()
    {
        var client = new HttpMockFactory()
            .Respond(HttpStatusCode.OK, """[{"bookingid":1},{"bookingid":7},{"bookingid":12}]""")
            .CreateClient();

        var ids = await client.GetBookingIdsAsync();

        ids.Should().Equal(1, 7, 12);
    }

    [Test]
    public async Task CreateBookingAsync_SerializesPayloadWithApiContractNames()
    {
        var factory = new HttpMockFactory().Respond(HttpStatusCode.OK, """
            {"bookingid":7,"booking":{"firstname":"Anna","lastname":"Smith","totalprice":200,
             "depositpaid":false,"bookingdates":{"checkin":"2026-07-01","checkout":"2026-07-04"}}}
            """);
        var client = factory.CreateClient();
        var booking = TestDataGenerator.DefaultBooking("Anna");

        var created = await client.CreateBookingAsync(booking);

        created.BookingId.Should().Be(7);
        var body = factory.Requests.Single().Body!;
        // The API contract requires all-lowercase keys and ISO dates.
        body.Should().Contain("\"firstname\":\"Anna\"")
            .And.Contain("\"bookingdates\"")
            .And.MatchRegex("\"checkin\":\"\\d{4}-\\d{2}-\\d{2}\"");
    }

    [Test]
    public async Task UpdateBookingAsync_SendsAuthTokenAsCookie()
    {
        var factory = new HttpMockFactory()
            .Respond(HttpStatusCode.OK, """{"token":"tok-1"}""")
            .Respond(HttpStatusCode.OK, """
                {"firstname":"Jim","lastname":"Brown","totalprice":1,"depositpaid":true,
                 "bookingdates":{"checkin":"2026-01-01","checkout":"2026-01-02"}}
                """);
        var client = factory.CreateClient();
        await client.AuthenticateAsync("admin", "password123");

        await client.UpdateBookingAsync(5, TestDataGenerator.DefaultBooking());

        var updateRequest = factory.Requests[1];
        updateRequest.Method.Should().Be(HttpMethod.Put);
        updateRequest.Uri.Should().Be($"{HttpMockFactory.BaseAddress}booking/5");
        updateRequest.Headers.Should().ContainKey("Cookie").WhoseValue.Should().Be("token=tok-1");
    }

    [Test]
    public async Task UpdateBookingAsync_WithoutToken_Throws()
    {
        var client = new HttpMockFactory().Respond(HttpStatusCode.OK).CreateClient();

        var act = () => client.UpdateBookingAsync(5, TestDataGenerator.DefaultBooking());

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*AuthenticateAsync*");
    }

    [Test]
    public async Task DeleteBookingAsync_Accepts201Created()
    {
        // Successful delete returns 201 — documented quirk of the API.
        var factory = new HttpMockFactory()
            .Respond(HttpStatusCode.OK, """{"token":"tok-1"}""")
            .Respond(HttpStatusCode.Created, "Created");
        var client = factory.CreateClient();
        await client.AuthenticateAsync("admin", "password123");

        var act = () => client.DeleteBookingAsync(5);

        await act.Should().NotThrowAsync();
    }

    [Test]
    public async Task DeleteBookingAsync_OnForbidden_ThrowsApiExceptionWithStatus()
    {
        var factory = new HttpMockFactory()
            .Respond(HttpStatusCode.OK, """{"token":"tok-1"}""")
            .Respond(HttpStatusCode.Forbidden, "Forbidden");
        var client = factory.CreateClient();
        await client.AuthenticateAsync("admin", "password123");

        var act = () => client.DeleteBookingAsync(5);

        (await act.Should().ThrowAsync<ApiException>())
            .Which.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task CreateBookingAsync_OnServerError_ThrowsApiExceptionWithBody()
    {
        var client = new HttpMockFactory()
            .Respond(HttpStatusCode.InternalServerError, "boom")
            .CreateClient();

        var act = () => client.CreateBookingAsync(TestDataGenerator.DefaultBooking());

        var exception = (await act.Should().ThrowAsync<ApiException>()).Which;
        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ResponseBody.Should().Be("boom");
        exception.Message.Should().Contain("500");
    }

    [Test]
    public async Task EveryRequest_SendsAcceptApplicationJson()
    {
        // Without an Accept header restful-booker rejects requests with 418 "I'm a Teapot".
        var factory = new HttpMockFactory().Respond(HttpStatusCode.OK, "[]");
        var client = factory.CreateClient();

        await client.GetBookingIdsAsync();

        factory.Requests.Single().Headers.Should()
            .ContainKey("Accept").WhoseValue.Should().Contain("application/json");
    }

    [Test]
    public async Task PingAsync_Returns_TrueOn201_FalseOtherwise()
    {
        var healthy = new HttpMockFactory().Respond(HttpStatusCode.Created).CreateClient();
        var broken = new HttpMockFactory().Respond(HttpStatusCode.ServiceUnavailable).CreateClient();

        (await healthy.PingAsync()).Should().BeTrue();
        (await broken.PingAsync()).Should().BeFalse();
    }
}
