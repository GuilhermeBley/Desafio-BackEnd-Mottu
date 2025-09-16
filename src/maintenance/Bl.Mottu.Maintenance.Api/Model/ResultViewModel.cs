using System.Text.Json.Serialization;

namespace Bl.Mottu.Maintenance.Api.Model;

public class ResultViewModel
{
    [JsonPropertyName("mensagem")]
    public string? Message { get; set; }

    public ResultViewModel() { }
    public ResultViewModel(string? message)
    {
        Message = message;
    }
}
