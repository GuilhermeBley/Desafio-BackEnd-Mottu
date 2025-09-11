
using Bl.Mottu.Maintenance.Core.Entities;
using Bl.Mottu.Maintenance.Core.Repository;

namespace Bl.Mottu.Maintenance.Core.Commands.CreateDeliveryDriver;

public record CreateDeliveryDriverRequest(
    string Name,
    string Cnpj,
    DateOnly BirthDate,
    string CnhNumber,
    string CnhCategory)
    : IRequest<CreateDeliveryDriverResponse>;

public record CreateDeliveryDriverResponse();

public class CreateDeliveryDriverHandler : IRequestHandler<CreateDeliveryDriverRequest, CreateDeliveryDriverResponse>
{
    private readonly DataContext _context;

    public Task<CreateDeliveryDriverResponse> Handle(CreateDeliveryDriverRequest request, CancellationToken cancellationToken)
    {
        DeliveryDriver.Create()
    }
}
