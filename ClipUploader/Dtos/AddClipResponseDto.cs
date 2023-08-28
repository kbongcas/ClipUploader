using Newtonsoft.Json;

namespace ClipUploader.Dtos;

public class AddClipResponseDto
{
    [JsonProperty("id")]
    public string ClipId { get; set; }
}
