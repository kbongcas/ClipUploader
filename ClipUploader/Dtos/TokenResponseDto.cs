using Newtonsoft.Json;

namespace ClipUploader.Dtos;

public class TokenResponseDto
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("token_type")]
    public string TokenType { get; set; }
}
