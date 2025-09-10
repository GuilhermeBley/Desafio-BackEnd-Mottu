
namespace Bl.Mottu.Maintenance.Core.Entities;

public class Motorcycle
{
    public Guid Id { get; private set; }
    public string Placa { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Motorcycle() { }

    public override bool Equals(object? obj)
    {
        return obj is Motorcycle motorcycle &&
               Id.Equals(motorcycle.Id) &&
               Placa == motorcycle.Placa &&
               Model == motorcycle.Model &&
               Year == motorcycle.Year &&
               CreatedAt == motorcycle.CreatedAt;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Placa, Model, Year, CreatedAt);
    }

    public static Result<Motorcycle> Create(
        string placa,
        string model,
        int year,
        DateTime? createdAt = null,
        Guid? id = null)
    {
        ResultBuilder<Motorcycle> builder = new();

        placa = placa ?? string.Empty;
        placa = string.Concat(placa.Where(char.IsLetterOrDigit));
        model = model ?? string.Empty;
        model = model.Trim().ToUpperInvariant();

        builder.AddIf(year < 1900, CoreExceptionCode.InvalidYear);
        builder.AddIf(placa.Length != 7, CoreExceptionCode.InvalidPlate);
        builder.AddIf(model.Length < 2 || model.Length > 250, CoreExceptionCode.InvalidModel);

        return builder.CreateResult(() =>
        {
            return new Motorcycle()
            {
                CreatedAt = createdAt ?? DateTime.UtcNow,
                Id= id ?? Guid.NewGuid(),
                Model = model,
                Year = year,
                Placa = placa,
            };
        });
    }
}
