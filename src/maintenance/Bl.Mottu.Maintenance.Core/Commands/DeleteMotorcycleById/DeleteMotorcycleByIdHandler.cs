using Bl.Mottu.Maintenance.Core.Model;
using Bl.Mottu.Maintenance.Core.Repository;

namespace Bl.Mottu.Maintenance.Core.Commands.DeleteMotorcycleById;

public record DeleteMotorcycleByIdResponse(Result Result); 

public record DeleteMotorcycleByIdRequest(string Code)
    : IRequest<DeleteMotorcycleByIdResponse>;

public class DeleteMotorcycleByIdHandler : IRequestHandler<DeleteMotorcycleByIdRequest, DeleteMotorcycleByIdResponse>
{
    private readonly DataContext _context;

    public DeleteMotorcycleByIdHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<DeleteMotorcycleByIdResponse> Handle(DeleteMotorcycleByIdRequest request, CancellationToken cancellationToken)
    {
        var modelFound = await _context.Motorcycles
            .Where(x => x.Code == request.Code)
            .AsNoTracking()
            .Select(x => new { x.Id })
            .FirstOrDefaultAsync(cancellationToken);

        if (modelFound == null) return new(Result.Failed(CoreExceptionCode.NotFound));

        await _context.Motorcycles
            .Where(x => x.Id == modelFound.Id)
            .ExecuteDeleteAsync(cancellationToken);

        return new(Result.Success());
    }
}
