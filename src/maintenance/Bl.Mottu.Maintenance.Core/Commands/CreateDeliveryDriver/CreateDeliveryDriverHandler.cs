
using Bl.Mottu.Maintenance.Core.Entities;
using Bl.Mottu.Maintenance.Core.Model;
using Bl.Mottu.Maintenance.Core.Repository;
using System.Xml.Linq;

namespace Bl.Mottu.Maintenance.Core.Commands.CreateDeliveryDriver;

public record CreateDeliveryDriverRequest(
    string Code,
    string Name,
    string Cnpj,
    DateOnly BirthDate,
    string CnhNumber,
    string CnhCategory,
    Stream? CnhImage = null)
    : IRequest<CreateDeliveryDriverResponse>;

public record CreateDeliveryDriverResponse(Result<DeliveryDriverModel> Result);

public class CreateDeliveryDriverHandler : IRequestHandler<CreateDeliveryDriverRequest, CreateDeliveryDriverResponse>
{
    private readonly DataContext _context;
    private readonly IStreamFileRepository _streamFileRepository;

    public CreateDeliveryDriverHandler(DataContext context, IStreamFileRepository streamFileRepository)
    {
        _context = context;
        _streamFileRepository = streamFileRepository;
    }

    public async Task<CreateDeliveryDriverResponse> Handle(CreateDeliveryDriverRequest request, CancellationToken cancellationToken)
    {
        var result = DeliveryDriver.Create(
            name: request.Name,
            code: request.Code,
            cnpj: request.Cnpj,
            birthdate: request.BirthDate,
            cnhNumber: request.CnhNumber,
            cnhKind: request.CnhCategory,
            cnhImageUrl: null);

        if (result.IsFailure)
        {
            return new(result.Cast<DeliveryDriverModel>());
        }

        var hasAnyByCnh =
            await _context
            .DeliveryDrivers
            .AsNoTracking()
            .Where(x => x.CnhNumber == result.RequiredResult.CnhNumber)
            .AnyAsync(cancellationToken);

        if (hasAnyByCnh) 
        {
            return new(Result.Failed<DeliveryDriverModel>(CoreExceptionCode.Conflict));        
        }

        var hasAnyByCode =
            await _context
            .DeliveryDrivers
            .AsNoTracking()
            .Where(x => x.Code == result.RequiredResult.Code)
            .AnyAsync(cancellationToken);

        if (hasAnyByCode)
        {
            return new(Result.Failed<DeliveryDriverModel>(CoreExceptionCode.Conflict));
        }

        var hasAnyByCnpj =
            await _context
            .DeliveryDrivers
            .AsNoTracking()
            .Where(x => x.Cnpj == result.RequiredResult.Cnpj)
            .AnyAsync(cancellationToken);

        if (hasAnyByCnpj)
        {
            return new(Result.Failed<DeliveryDriverModel>(CoreExceptionCode.Conflict));
        }

        await using var transaction 
            = await _context.Database.BeginTransactionAsync(cancellationToken);

        var response = await _context.DeliveryDrivers
            .AddAsync(DeliveryDriverModel.MapFromEntity(result.RequiredResult));

        await _context.SaveChangesAsync(cancellationToken);


        if (request.CnhImage is not null)
        {
            var uri = await _streamFileRepository.UploadAsync(request.CnhImage, $"{response.Entity.Id}-cnh.png", cancellationToken);
            await _context
                .DeliveryDrivers
                .Where(x => x.Id == response.Entity.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.CnhImg, uri.AbsoluteUri));
        }

        await transaction.CommitAsync();

        return new(
            Result.Success(response.Entity));
    }
}
