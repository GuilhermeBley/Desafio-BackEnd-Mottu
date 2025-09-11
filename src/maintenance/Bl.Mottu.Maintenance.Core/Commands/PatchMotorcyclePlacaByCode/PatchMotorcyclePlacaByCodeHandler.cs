using Bl.Mottu.Maintenance.Core.Entities;
using Bl.Mottu.Maintenance.Core.Repository;

namespace Bl.Mottu.Maintenance.Core.Commands.PatchMotorcyclePlacaByCode;
public record PatchMotorcyclePlacaByCodeRequest(
    string Code,
    string Placa)
    : IRequest<PatchMotorcyclePlacaByCodeResponse>;

public record PatchMotorcyclePlacaByCodeResponse(Result Result);

public class PatchMotorcyclePlacaByCodeHandler : IRequestHandler<PatchMotorcyclePlacaByCodeRequest, PatchMotorcyclePlacaByCodeResponse>
{
    private readonly DataContext _context;

    public PatchMotorcyclePlacaByCodeHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<PatchMotorcyclePlacaByCodeResponse> Handle(PatchMotorcyclePlacaByCodeRequest request, CancellationToken cancellationToken)
    {
        var placa = Motorcycle.NormalizePlaca(request.Placa);

        if (string.IsNullOrEmpty(placa)) return new(Result.Failed(CoreExceptionCode.InvalidPlate));

        var modelFound = 
            await _context
            .Motorcycles
            .Where(x => x.Code == request.Code)
            .AsTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (modelFound  == null) return new(Result.Failed(CoreExceptionCode.NotFound));

        modelFound.Placa = placa;

        await _context.SaveChangesAsync(cancellationToken);

        return new(Result.Success(placa));
    }
}
