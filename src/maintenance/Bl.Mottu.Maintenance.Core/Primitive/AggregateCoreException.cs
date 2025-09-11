using System.Collections.Immutable;

namespace Bl.Mottu.Maintenance.Core.Primitive;

public class AggregateCoreException : CoreException, ICoreException
{
    private readonly ImmutableArray<ICoreException> _errors;
    public override CoreExceptionCode StatusCode => CoreExceptionCode.BadRequest;
    public IReadOnlyList<ICoreException> InnerExceptions => _errors;

    public AggregateCoreException()
    {
        _errors = ImmutableArray<ICoreException>.Empty;
    }

    public AggregateCoreException(string? message) : base(message)
    {
        _errors = ImmutableArray<ICoreException>.Empty;
    }

    public AggregateCoreException(string? message, Exception? innerException) : base(message, innerException)
    {
        _errors = ImmutableArray<ICoreException>.Empty;
    }

    public AggregateCoreException(IEnumerable<ICoreException> errors)
    {
        _errors = errors.ToImmutableArray();
    }

    public AggregateCoreException(string? message, IEnumerable<ICoreException> errors) : base(message)
    {
        _errors = errors.ToImmutableArray();
    }

    public AggregateCoreException(string? message, Exception? innerException, IEnumerable<ICoreException> errors) : base(message, innerException)
    {
        _errors = errors.ToImmutableArray();
    }
}
