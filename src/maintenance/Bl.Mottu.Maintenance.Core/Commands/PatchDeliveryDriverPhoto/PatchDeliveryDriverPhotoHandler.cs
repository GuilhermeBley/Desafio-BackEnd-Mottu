
using Bl.Mottu.Maintenance.Core.Entities;
using Bl.Mottu.Maintenance.Core.Repository;

namespace Bl.Mottu.Maintenance.Core.Commands.PatchDeliveryDriverPhoto;

public record PatchDeliveryDriverPhotoRequest(
    string Code,
    Stream? CnhImage)
    : IRequest<PatchDeliveryDriverPhotoResponse>;

public record PatchDeliveryDriverPhotoResponse(Result Result);

public class PatchDeliveryDriverPhotoHandler : IRequestHandler<PatchDeliveryDriverPhotoRequest, PatchDeliveryDriverPhotoResponse>
{
    private readonly DataContext _context;
    private readonly IStreamFileRepository _streamFileRepository;

    public PatchDeliveryDriverPhotoHandler(DataContext context, IStreamFileRepository streamFileRepository)
    {
        _context = context;
        _streamFileRepository = streamFileRepository;
    }

    public async Task<PatchDeliveryDriverPhotoResponse> Handle(PatchDeliveryDriverPhotoRequest request, CancellationToken cancellationToken)
    {
        var code = new CodeId(request.Code);

        var modelFound = await
            _context
            .DeliveryDrivers
            .AsNoTracking()
            .Where(x => x.Code == code.ToString())
            .Select(x => new { x.Id })
            .FirstOrDefaultAsync(cancellationToken);

        if (modelFound is null)
        {
            return new(Result.Failed(CoreExceptionCode.NotFound));
        }

        await using var transaction = 
            await _context.Database.BeginTransactionAsync(cancellationToken);

        if (request.CnhImage != null)
        {
            var fileUri =
                await _streamFileRepository.UploadAsync(request.CnhImage, $"{code}-cnh.png", cancellationToken);

            await _context
                .DeliveryDrivers
                .Where(x => x.Code == code.ToString())
                .Take(1)
                .ExecuteUpdateAsync(p => 
                    p.SetProperty(x => x.CnhImg, fileUri.AbsoluteUri));
        }

        await _context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync();

        return new(Result.Success());
    }
}
