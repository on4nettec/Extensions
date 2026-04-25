using System.Net;
using Newtonsoft.Json;
using On4Net.Extensions.Identity.Firebase.Models;
using On4Net.Extensions.Identity.Firebase.Services;
using Xunit;

namespace On4Net.Extensions.Tests.Firebase;

/// <summary>
/// HTTP-level tests for <see cref="FirebaseServices"/> without calling Google.
/// </summary>
public sealed class FirebaseServicesTests
{
    [Fact]
    public async Task Login_returns_model_on_success()
    {
        var payload = new LoginModel
        {
            IdToken = "t",
            Email = "e@e.com",
            RefreshToken = "r",
            ExpiresIn = "3600",
            LocalId = "lid",
            Registered = true
        };
        var json = JsonConvert.SerializeObject(payload);

        var handler = new FakeHttpMessageHandler((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) }));

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://identitytoolkit.googleapis.com/v1/") };
        var services = new FirebaseServices(client, "api-key");

        var result = await services.Login("e@e.com", "secret", CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal("t", result.IdToken);
    }

    [Fact]
    public async Task Login_returns_null_on_http_error()
    {
        var handler = new FakeHttpMessageHandler((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://identitytoolkit.googleapis.com/v1/") };
        var services = new FirebaseServices(client, "k");
        var result = await services.Login("a", "b", CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task SignupNewUser_deserializes_login_model()
    {
        var json = JsonConvert.SerializeObject(new LoginModel { IdToken = "x" });
        var handler = new FakeHttpMessageHandler((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) }));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://identitytoolkit.googleapis.com/v1/") };
        var services = new FirebaseServices(client, "k");
        var result = await services.SignupNewUser("a@a.com", "pw", true, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal("x", result.IdToken);
    }

    [Fact]
    public async Task SendVerifyEmail_returns_body_string()
    {
        var handler = new FakeHttpMessageHandler((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") }));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://identitytoolkit.googleapis.com/v1/") };
        var services = new FirebaseServices(client, "k");
        var body = await services.SendVerifyEmail("token", CancellationToken.None);
        Assert.Equal("ok", body);
    }

    [Fact]
    public async Task VerifyCustomToken_deserializes_model()
    {
        var json = JsonConvert.SerializeObject(new VerifyCustomTokenModel { IdToken = "it", RefreshToken = "rt", ExpiresIn = "1" });
        var handler = new FakeHttpMessageHandler((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) }));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://identitytoolkit.googleapis.com/v1/") };
        var services = new FirebaseServices(client, "k");
        var m = await services.VerifyCustomToken("ct", true, CancellationToken.None);
        Assert.NotNull(m);
        Assert.Equal("it", m.IdToken);
    }
}
