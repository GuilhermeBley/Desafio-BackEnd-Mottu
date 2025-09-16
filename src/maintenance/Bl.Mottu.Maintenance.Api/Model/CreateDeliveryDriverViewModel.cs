using System.Text.Json.Serialization;

namespace Bl.Mottu.Maintenance.Api.Model;

public class CreateDeliveryDriverViewModel
{
    [JsonPropertyName("identificador")]
    public string Identificador { get; set; } = string.Empty;

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("cnpj")]
    public string Cnpj { get; set; } = string.Empty;

    [JsonPropertyName("data_nascimento")]
    public DateTime DataNascimento { get; set; }

    [JsonPropertyName("numero_cnh")]
    public string NumeroCnh { get; set; } = string.Empty;

    [JsonPropertyName("tipo_cnh")]
    public string TipoCnh { get; set; } = string.Empty;

    [JsonPropertyName("imagem_cnh")]
    public string ImagemCnh { get; set; } = string.Empty;

    public Stream? GetCnhImage()
    {
        if (string.IsNullOrWhiteSpace(ImagemCnh)) return null;

        try
        {
            byte[] bytes = Convert.FromBase64String(ImagemCnh);
            return new MemoryStream(bytes);
        }
        catch
        {
            return null;
        }
    }
}
