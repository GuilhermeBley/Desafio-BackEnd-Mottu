using Bl.Mottu.Maintenance.Core.Entities;
using Bl.Mottu.Maintenance.Core.Model;
using Bl.Mottu.Maintenance.Core.Repository;

namespace Bl.Mottu.Maintenance.Core.Commands.GetMotorcycles;

public record GetMotorcyclesRequest(string? CodeFilter, string? PlacaFilter)
    : IRequest<GetMotorcyclesResponse>;

public record GetMotorcyclesResponse(MotorcycleModel[] Results);

public class GetMotorcyclesHandler : IRequestHandler<GetMotorcyclesRequest, GetMotorcyclesResponse>
{
    private readonly DataContext _context;

    public GetMotorcyclesHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<GetMotorcyclesResponse> Handle(GetMotorcyclesRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.PlacaFilter) is false)
        {
            var placaFilter = Motorcycle.NormalizePlaca(request.PlacaFilter);
            var filteredModels = await 
                _context
                .Motorcycles
                .Where(x => x.Code == placaFilter)
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new(filteredModels);
        }

        if (string.IsNullOrEmpty(request.CodeFilter) is false)
        {
            var filteredModels = await 
                _context
                .Motorcycles
                .Where(x => x.Code == request.CodeFilter)
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new(filteredModels);
        }

        var models = await
            _context
            .Motorcycles
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        return new(models);
    }
}
