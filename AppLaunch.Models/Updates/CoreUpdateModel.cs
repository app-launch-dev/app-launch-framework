using System.Text.Json.Serialization;

namespace AppLaunch.Models.Updates;

public class CoreUpdateModel
{
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; set; }
}