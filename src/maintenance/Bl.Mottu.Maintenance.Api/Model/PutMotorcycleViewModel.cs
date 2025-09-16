using System.Text.Json.Serialization;

namespace Bl.Mottu.Maintenance.Api.Model;

public class PutMotorcycleViewModel
{
    [JsonPropertyName("placa")]
    public string? Placa { get; set; }
}
