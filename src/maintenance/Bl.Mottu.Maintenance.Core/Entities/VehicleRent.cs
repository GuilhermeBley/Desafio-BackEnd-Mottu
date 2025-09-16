namespace Bl.Mottu.Maintenance.Core.Entities;

public class VehicleRent
{
    // TODO: Change it to a database table
    private static readonly IReadOnlyDictionary<int, decimal> _planDailyValues = new Dictionary<int, decimal>()
    {
        {7, 30},
        {15, 28},
        {30, 22},
        {45, 20},
        {50 ,18},
    };

    public Guid Id { get; private set; }
    public Guid DeliveryDriverId { get; private set; }
    public Guid VehicleId { get; private set; }
    public DateTime StartAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public DateTime ExpectedEndingDate { get; private set; }
    public int Plan { get; private set; }
    public decimal DailyValue { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private VehicleRent() { }

    public override bool Equals(object? obj)
    {
        return obj is VehicleRent rent &&
               Id.Equals(rent.Id) &&
               DeliveryDriverId.Equals(rent.DeliveryDriverId) &&
               VehicleId.Equals(rent.VehicleId) &&
               StartAt == rent.StartAt &&
               EndedAt == rent.EndedAt &&
               ExpectedEndingDate == rent.ExpectedEndingDate &&
               Plan == rent.Plan &&
               DailyValue == rent.DailyValue &&
               CreatedAt == rent.CreatedAt;
    }

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        hash.Add(Id);
        hash.Add(DeliveryDriverId);
        hash.Add(VehicleId);
        hash.Add(StartAt);
        hash.Add(EndedAt);
        hash.Add(ExpectedEndingDate);
        hash.Add(Plan);
        hash.Add(DailyValue);
        hash.Add(CreatedAt);
        return hash.ToHashCode();
    }

    public static Result<VehicleRent> Create(
        Guid deliveryDriverId,
        Guid vehicleId,
        DateTime startAt,
        DateTime expectedEndingDate,
        int plan,
        DateTime? endedAt = null,
        DateTime? createdAt = null,
        Guid? id = null)
    {
        ResultBuilder<VehicleRent> builder = new();

        builder.AddIf(startAt == DateTime.MinValue, CoreExceptionCode.InvalidStartDate);
        builder.AddIf(expectedEndingDate == DateTime.MinValue, CoreExceptionCode.InvalidEstimatedEndDate);

        builder.AddIf(startAt >= expectedEndingDate, CoreExceptionCode.InvalidRentalPeriod);
        builder.AddIf(endedAt.HasValue && endedAt.Value < startAt, CoreExceptionCode.InvalidEndDate);
        builder.AddIf(endedAt.HasValue && endedAt.Value > expectedEndingDate, CoreExceptionCode.LateReturn);

        builder.AddIf(plan <= 0, CoreExceptionCode.InvalidRentalPlan);
        builder.AddIf(_planDailyValues.TryGetValue(plan, out var dailyValue) is false, CoreExceptionCode.InvalidRentalPlan);

        var expectedEndDate = startAt.AddDays(plan);
        builder.AddIf(expectedEndingDate.Date < expectedEndDate.Date, CoreExceptionCode.PlanMismatch);

        return builder.CreateResult(() =>
        {
            return new VehicleRent()
            {
                Id = id ?? Guid.NewGuid(),
                DeliveryDriverId = deliveryDriverId,
                VehicleId = vehicleId,
                StartAt = startAt,
                EndedAt = endedAt,
                ExpectedEndingDate = expectedEndingDate,
                Plan = plan,
                DailyValue = dailyValue,
                CreatedAt = createdAt ?? DateTime.UtcNow
            };
        });
    }
}