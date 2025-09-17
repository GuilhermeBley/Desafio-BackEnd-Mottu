using Bl.Mottu.Maintenance.Core.Entities;
using Bl.Mottu.Maintenance.Core.Events;
using Bl.Mottu.Maintenance.Core.Model;
using Bl.Mottu.Maintenance.Core.Repository;

namespace Bl.Mottu.Maintenance.Core.Commands.CreateMotorcycle;

public record CreateMotorcycleResponse(Result<MotorcycleModel> Result);

public record CreateMotorcycleRequest(
    string Code,
    string Placa,
    string Model,
    int Year)
    : IRequest<CreateMotorcycleResponse>;

public class CreateMotorcycleHandler : IRequestHandler<CreateMotorcycleRequest, CreateMotorcycleResponse>
{
    private readonly DataContext _context;
    private readonly IEventBus _bus;

    public CreateMotorcycleHandler(DataContext context, IEventBus bus)
    {
        _context = context;
        _bus = bus;
    }

    public async Task<CreateMotorcycleResponse> Handle(CreateMotorcycleRequest request, CancellationToken cancellationToken)
    {
        var motorcycleResult = Motorcycle.Create(
            placa: request.Placa,
            model: request.Model,
            code: request.Code,
            year: request.Year);
        
        if (motorcycleResult.IsFailure) return new(motorcycleResult.Cast<MotorcycleModel>());

        var motorcycle = motorcycleResult.RequiredResult;

        var alreadyExists = await _context
            .Motorcycles
            .Where(x => x.Placa == request.Placa || x.Code == request.Code)
            .AsNoTracking()
            .AnyAsync(cancellationToken);

        if (alreadyExists) return new(Result.Failed<MotorcycleModel>(CoreExceptionCode.Conflict));

        var result = await _context.Motorcycles.AddAsync(
            MotorcycleModel.MapFromEntity(motorcycle));
        await _context.SaveChangesAsync();

        // TODO: publish an event on creating motorcycle
        //await _bus.PublishAsync(new CreatedMotorcycleEvent()
        //{
        //    Placa = result.Entity.Placa,
        //    Year = result.Entity.Year,
        //});

        return new(Result.Success(result.Entity));
    }
}
