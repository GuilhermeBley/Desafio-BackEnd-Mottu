using System.Text.Json.Serialization;

namespace Bl.Mottu.Maintenance.Api.Model;

public class CreateCnhDeliveryDriverViewModel
{

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
