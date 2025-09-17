namespace Bl.Mottu.Maintenance.Core.Events;
public class IntegrationEvent
{
    /// <summary>
    /// Additional filters
    /// </summary>
    public virtual IReadOnlyDictionary<string, object> Filters { get; } = new Dictionary<string, object>();
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
