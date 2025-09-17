
namespace Bl.Mottu.Maintenance.Core.Events;

public class CreatedMotorcycleEvent : IntegrationEvent
{
    public string Placa { get; set; } = string.Empty;
    public int Year { get; set; }
    public override IReadOnlyDictionary<string, object> Filters => 
        new Dictionary<string, object>()
        {
            { nameof(Year), Year },
        };
}
