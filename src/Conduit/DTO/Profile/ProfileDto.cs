using System.Text.Json.Serialization;
namespace Conduit.DTO;

public record ProfileDto
{
    public string? Username { get; set; }

    public string? Bio { get; set; }

    public string? Image { get; set; }

    [JsonPropertyName("following")]
    public bool IsFollowed { get; set; }
}
