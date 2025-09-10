namespace Bl.Mottu.Maintenance.Core.Primitive;

public interface ICoreException
{
    /// <summary>
    /// Status code
    /// </summary>
    CoreExceptionCode StatusCode { get; }

    /// <summary>
    /// Message
    /// </summary>
    string? Message { get; }
}
