using Bl.Mottu.Maintenance.Core.Entities;
using Bl.Mottu.Maintenance.Core.Model;
using Bl.Mottu.Maintenance.Core.Repository;
using System.Numerics;

namespace Bl.Mottu.Maintenance.Core.Commands.CreateVehicleRent;

public record CreateVehicleRentResponse(Result<VehicleRentModel> Result);

public record CreateVehicleRentRequest(
    string DeliveryDriverCode,
    string VehicleCode,
    DateTime StartAt,
    DateTime? EndedAt,
    DateTime ExpectedEndingDate,
    int Plan)
    : IRequest<CreateVehicleRentResponse>;

public class CreateVehicleRentHandler : IRequestHandler<CreateVehicleRentRequest, CreateVehicleRentResponse>
{
    private readonly DataContext _context;

    public CreateVehicleRentHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<CreateVehicleRentResponse> Handle(CreateVehicleRentRequest request, CancellationToken cancellationToken)
    {
        var deliveryDriverCode = new CodeId(request.DeliveryDriverCode);
        var vehicleCode = new CodeId(request.VehicleCode);

        var vehicleFound = await _context
            .Motorcycles
            .AsNoTracking()
            .Where(x => x.Code == vehicleCode.ToString())
            .Select(x => new { x.Code, x.Id })
            .FirstOrDefaultAsync(cancellationToken);

        if (vehicleFound is null) return new(Result.Failed<VehicleRentModel>(CoreExceptionCode.NotFound));

        var deliveryDriverFound = 
            await _context
            .DeliveryDrivers
            .AsNoTracking()
            .Where(x => x.Code == deliveryDriverCode.ToString())
            .Select(x => new { x.Code, x.Id, x.CnhCategory })
            .FirstOrDefaultAsync(cancellationToken);

        if (deliveryDriverFound is null) return new(Result.Failed<VehicleRentModel>(CoreExceptionCode.NotFound));

        var entityResult = VehicleRent.Create(
            deliveryDriverId: deliveryDriverFound.Id,
            vehicleId: vehicleFound.Id,
            startAt: request.StartAt,
            expectedEndingDate: request.ExpectedEndingDate,
            plan: request.Plan,
            endedAt: request.EndedAt);

        if (entityResult.IsFailure) return new(entityResult.Cast<VehicleRentModel>());

        var entity = entityResult.RequiredResult;

        if (!deliveryDriverFound.CnhCategory.Contains('a', StringComparison.OrdinalIgnoreCase))
        {
            return new(Result.Failed<VehicleRentModel>(CoreExceptionCode.DriverHasInsufficientCategory));
        }

        var vehicleIsAlreadyRented =
            await _context
            .VehiclesRent
            .AsNoTracking()
            .Where(x => x.VehicleId == vehicleFound.Id)
            .Where(x => 
                (request.StartAt >= x.StartAt && request.StartAt <= x.ExpectedEndingDate) || // checking if the start is in the range
                (request.StartAt < x.StartAt && request.ExpectedEndingDate >= x.ExpectedEndingDate))// checking if the ending is in the range
            .AnyAsync(cancellationToken);

        if (vehicleIsAlreadyRented)
        {
            return new(Result.Failed<VehicleRentModel>(CoreExceptionCode.Conflict));
        }

        var addingResult = await _context
            .VehiclesRent
            .AddAsync(VehicleRentModel.MapFromEntity(entity), cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new(Result.Success(addingResult.Entity));
    }
}
