
using System.Numerics;

namespace Bl.Mottu.Maintenance.Core.Entities;

public class Motorcycle
{
    public Guid Id { get; private set; }
    public string Placa { get; private set; } = string.Empty;
    public CodeId Code { get; private set; }
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Motorcycle() { }

    public override bool Equals(object? obj)
    {
        return obj is Motorcycle motorcycle &&
               Id.Equals(motorcycle.Id) &&
               Placa == motorcycle.Placa &&
               Code == motorcycle.Code &&
               Model == motorcycle.Model &&
               Year == motorcycle.Year &&
               CreatedAt == motorcycle.CreatedAt;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Placa, Model, Year, Code, CreatedAt);
    }

    public static Result<Motorcycle> Create(
        string placa,
        string model,
        string code,
        int year,
        DateTime? createdAt = null,
        Guid? id = null)
    {
        ResultBuilder<Motorcycle> builder = new();

        placa = placa ?? string.Empty;
        placa = NormalizePlaca(placa);
        model = model ?? string.Empty;
        code = code ?? string.Empty;
        model = model.Trim().ToUpperInvariant();

        builder.AddIf(year < 1900, CoreExceptionCode.InvalidYear);
        builder.AddIf(placa.Length != 7, CoreExceptionCode.InvalidPlate);
        builder.AddIf(model.Length < 2 || model.Length > 250, CoreExceptionCode.InvalidModel);

        return builder.CreateResult(() =>
        {
            return new Motorcycle()
            {
                Code = new(code),
                CreatedAt = createdAt ?? DateTime.UtcNow,
                Id= id ?? Guid.NewGuid(),
                Model = model,
                Year = year,
                Placa = placa,
            };
        });
    }

    public static string NormalizePlaca(string? i)
    {
        if (string.IsNullOrEmpty(i)) return string.Empty;
        var plate = string.Concat(i.Where(char.IsLetterOrDigit));

        if (plate.Length != 7) return string.Empty;

        return plate;
    }
}
