using Bl.Mottu.Maintenance.Core.Entities;

namespace Bl.Mottu.Maintenance.Core.Model;

public class MotorcycleModel
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public DateTime CreatedAt { get; set; }

    public static MotorcycleModel MapFromEntity(Motorcycle entity)
    {
        return new MotorcycleModel()
        {
            CreatedAt = entity.CreatedAt,
            Model = entity.Model,
            Code = entity.Code,
            Id = entity.Id,
            Placa = entity.Placa,
        };
    }
}
