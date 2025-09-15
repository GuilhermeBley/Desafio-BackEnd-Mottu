using Bl.Mottu.Maintenance.Core.ValueObjects;

namespace Bl.Mottu.Maintenance.Core.Entities;

public class DeliveryDriver
{
    public Guid Id { get; private set; }
    public CodeId Code { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Cnpj { get; private set; } = string.Empty;
    public DateOnly BirthDate { get; private set; }
    public string CnhNumber { get; private set; } = string.Empty;
    public string CnhCategory { get; private set; } = string.Empty;
    public Uri? CnhImgUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private DeliveryDriver() { }

    public override bool Equals(object? obj)
    {
        return obj is DeliveryDriver driver &&
               Id.Equals(driver.Id) &&
               Code == driver.Code &&
               Name == driver.Name &&
               Cnpj == driver.Cnpj &&
               BirthDate == driver.BirthDate &&
               CnhNumber == driver.CnhNumber &&
               CnhCategory == driver.CnhCategory &&
               CnhImgUrl == driver.CnhImgUrl &&
               CreatedAt == driver.CreatedAt;
    }

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        hash.Add(Id);
        hash.Add(Code);
        hash.Add(Name);
        hash.Add(Cnpj);
        hash.Add(BirthDate);
        hash.Add(CnhNumber);
        hash.Add(CnhCategory);
        hash.Add(CnhImgUrl);
        hash.Add(CreatedAt);
        return hash.ToHashCode();
    }

    public static Result<DeliveryDriver> Create(
        string code,
        string name,
        string cnpj,
        DateOnly birthdate,
        string cnhNumber,
        string cnhKind,
        string? cnhImageUrl,
        DateTime? createdAt = null,
        Guid? id = null)
    {
        ResultBuilder<DeliveryDriver> builder = new();

        name = (name?.Trim() ?? string.Empty).ToUpperInvariant();
        cnpj = cnpj?.Trim() ?? string.Empty;
        cnhNumber = cnhNumber?.Trim() ?? string.Empty;
        cnhNumber = string.Concat(cnhNumber.Where(char.IsNumber));
        cnpj = string.Concat(cnpj.Where(char.IsNumber));
        cnhKind = cnhKind?.Trim().ToUpperInvariant() ?? string.Empty;
        cnhImageUrl = string.IsNullOrWhiteSpace(cnhImageUrl) ? null : cnhImageUrl;

        builder.AddIf(string.IsNullOrWhiteSpace(name), CoreExceptionCode.InvalidName);
        builder.AddIf(name.Length < 2 || name.Length > 250, CoreExceptionCode.InvalidName);

        builder.AddIf(string.IsNullOrWhiteSpace(cnpj), CoreExceptionCode.InvalidCnpj);
        builder.AddIf(cnpj.Length != 14 || !cnpj.All(char.IsDigit), CoreExceptionCode.InvalidCnpj);

        builder.AddIf(birthdate.Year < 1900, CoreExceptionCode.InvalidBirthDate);

        builder.AddIf(string.IsNullOrWhiteSpace(cnhNumber), CoreExceptionCode.InvalidCnhNumber);
        builder.AddIf(cnhNumber .Length != 11 || cnhNumber.All(c => c == '0'), CoreExceptionCode.InvalidCnhNumber);

        builder.AddIf(string.IsNullOrWhiteSpace(cnhKind), CoreExceptionCode.InvalidCnhType);
        builder.AddIf(!new[] { "A", "B", "AB" }.Contains(cnhKind), CoreExceptionCode.InvalidCnhType);

        Uri? cnhImageUri = null;
        if (cnhImageUrl != null)
            builder.AddIf(Uri.TryCreate(cnhImageUrl, UriKind.Absolute, out cnhImageUri) is false, CoreExceptionCode.InvalidCnhImage);

        return builder.CreateResult(() =>
        {
            return new DeliveryDriver()
            {
                Id = id ?? Guid.NewGuid(),
                Name = name,
                Cnpj = cnpj,
                BirthDate = birthdate,
                CnhNumber = cnhNumber,
                CnhCategory = cnhKind,
                Code = new(code),
                CnhImgUrl = cnhImageUri,
                CreatedAt = createdAt ?? DateTime.UtcNow
            };
        });
    }
}