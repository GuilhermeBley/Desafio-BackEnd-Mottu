using Bl.Mottu.Maintenance.Core.Model;
using Bl.Mottu.Maintenance.Core.Repository;

namespace Bl.Mottu.Maintenance.Core.Commands.GetVehicleRentById;

public record GetVehicleRentByIdRequest(Guid Id)
    : IRequest<GetVehicleRentByIdResponse>;

public record GetVehicleRentByIdResponse(VehicleRentModel? Result);

public class GetVehicleRentByIdHandler : IRequestHandler<GetVehicleRentByIdRequest, GetVehicleRentByIdResponse>
{
    private readonly DataContext _context;

    public GetVehicleRentByIdHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<GetVehicleRentByIdResponse> Handle(GetVehicleRentByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await _context
            .VehiclesRent
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return new(result);
    }
}
