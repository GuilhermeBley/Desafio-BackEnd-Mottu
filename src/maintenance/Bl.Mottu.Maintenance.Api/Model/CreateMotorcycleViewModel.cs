using System.Text.Json.Serialization;

namespace Bl.Mottu.Maintenance.Api.Model;

public class CreateMotorcycleViewModel
{
    [JsonPropertyName("identificador")]
    public string? Identificador { get; set; }

    [JsonPropertyName("ano")]
    public int Ano { get; set; }

    [JsonPropertyName("modelo")]
    public string? Modelo { get; set; }

    [JsonPropertyName("placa")]
    public string? Placa { get; set; }
}
