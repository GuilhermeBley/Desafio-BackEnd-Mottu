using Bl.Mottu.Maintenance.Core.Entities;

namespace Bl.Mottu.Maintenance.Core.Model;
public class DeliveryDriverModel
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string CnhNumber { get; set; } = string.Empty;
    public string CnhCategory { get; set; } = string.Empty;
    public string? CnhImg { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public static DeliveryDriverModel MapFromEntity(DeliveryDriver entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Cnpj = entity.Cnpj,
            BirthDate = entity.BirthDate,
            CnhCategory= entity.CnhCategory,
            CnhImg = entity.CnhImgUrl?.AbsoluteUri,
            CnhNumber = entity.CnhNumber,
            Code = entity.Code,
            CreatedAt = entity.CreatedAt,
        };
    }
}
