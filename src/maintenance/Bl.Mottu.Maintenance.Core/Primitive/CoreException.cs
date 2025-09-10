namespace Bl.Mottu.Maintenance.Core.Primitive;

/// <summary>
/// Exceptions in Core
/// </summary>
public abstract class CoreException : Exception, ICoreException
{
    /// <summary>
    /// Status code
    /// </summary>
    public abstract CoreExceptionCode StatusCode { get; }

    /// <summary>
    /// Source Core
    /// </summary>
    public override string? Source => "Bl.Gym.Domain";

    protected CoreException()
    {
    }

    protected CoreException(string? message) : base(message)
    {
    }

    protected CoreException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public override string ToString()
    {
        return $"{StatusCode}|{base.Message}";
    }

    public static CoreException CreateByCode(CoreExceptionCode code)
        => new InternalCoreException(code);

    private class InternalCoreException : CoreException
    {
        private readonly CoreExceptionCode _statusCode;
        public override CoreExceptionCode StatusCode => _statusCode;

        public InternalCoreException(CoreExceptionCode statusCode)
            : base(statusCode.ToString())
        {
            _statusCode = statusCode;
        }
    }
}
