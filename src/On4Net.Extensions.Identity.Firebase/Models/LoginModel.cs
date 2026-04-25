using Newtonsoft.Json;

namespace On4Net.Extensions.Identity.Firebase.Models;

public class LoginModel
{
    [JsonProperty("idToken")]
    public string IdToken { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("refreshToken")]
    public string RefreshToken { get; set; }

    [JsonProperty("expiresIn")]
    public string ExpiresIn { get; set; }

    [JsonProperty("localId")]
    public string LocalId { get; set; }

    [JsonProperty("registered")]
    public bool Registered { get; set; }
}