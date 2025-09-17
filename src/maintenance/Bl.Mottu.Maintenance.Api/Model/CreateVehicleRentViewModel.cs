using System.Text.Json.Serialization;

namespace Bl.Mottu.Maintenance.Api.Model;

public class CreateVehicleRentViewModel
{
    [JsonPropertyName("entregador_id")]
    public string? EntregadorId { get; set; }

    [JsonPropertyName("moto_id")]
    public string? MotoId { get; set; }

    [JsonPropertyName("data_inicio")]
    public DateTimeOffset DataInicio { get; set; }

    [JsonPropertyName("data_termino")]
    public DateTimeOffset? DataTermino { get; set; }

    [JsonPropertyName("data_previsao_termino")]
    public DateTimeOffset DataPrevisaoTermino { get; set; }

    [JsonPropertyName("plano")]
    public int Plano { get; set; }
}
