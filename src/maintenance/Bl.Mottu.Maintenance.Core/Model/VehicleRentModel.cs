using Bl.Mottu.Maintenance.Core.Entities;

namespace Bl.Mottu.Maintenance.Core.Model;

public class VehicleRentModel
{
    public Guid Id { get; set; }
    public Guid DeliveryDriverId { get; set; }
    public Guid VehicleId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public DateTime ExpectedEndingDate { get; set; }
    public int Plan { get; set; }
    public DateTime CreatedAt { get; set; }

    public static VehicleRentModel MapFromEntity(VehicleRent entity)
    {
        return new VehicleRentModel
        {
            Id = entity.Id,
            DeliveryDriverId = entity.DeliveryDriverId,
            VehicleId = entity.VehicleId,
            CreatedAt = entity.CreatedAt,
            EndedAt = entity.EndedAt,
            ExpectedEndingDate = entity.ExpectedEndingDate,
            Plan = entity.Plan,
            StartAt = entity.StartAt,

        };
    }
}
