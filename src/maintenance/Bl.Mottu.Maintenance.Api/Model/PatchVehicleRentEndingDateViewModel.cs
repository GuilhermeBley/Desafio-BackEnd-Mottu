using System.Text.Json.Serialization;

namespace Bl.Mottu.Maintenance.Api.Model;

public class PatchVehicleRentEndingDateViewModel
{

    [JsonPropertyName("data_devolucao")]
    public DateTimeOffset DataTermino { get; set; }
}
