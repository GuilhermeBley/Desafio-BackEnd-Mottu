
using Bl.Mottu.Maintenance.Core.Repository;

namespace Bl.Mottu.Maintenance.Core.Commands.PatchVehicleRentEndingDate;

public record PatchVehicleRentEndingDateRequest(
    Guid Id,
    DateTime DevolutionDate)
    : IRequest<PatchVehicleRentEndingDateResponse>;

public record PatchVehicleRentEndingDateResponse(Result Result);

public class PatchVehicleRentEndingDateHandler : IRequestHandler<PatchVehicleRentEndingDateRequest, PatchVehicleRentEndingDateResponse>
{
    private readonly DataContext _context;

    public PatchVehicleRentEndingDateHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<PatchVehicleRentEndingDateResponse> Handle(PatchVehicleRentEndingDateRequest request, CancellationToken cancellationToken)
    {
        var modelFound = await _context
            .VehiclesRent
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (modelFound is null) return new(Result.Failed(CoreExceptionCode.NotFound));

        await _context
            .VehiclesRent
            .Where(x => x.Id == request.Id)
            .ExecuteUpdateAsync(
                u => u.SetProperty(x => x.EndedAt, request.DevolutionDate),
                cancellationToken);

        return new(Result.Success());
    }
}
