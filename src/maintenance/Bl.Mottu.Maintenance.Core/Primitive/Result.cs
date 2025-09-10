using System.Diagnostics.CodeAnalysis;

namespace Bl.Mottu.Maintenance.Core.Primitive;

public class Result
{
    public const CoreExceptionCode DEFAULT_STATUS_ERROR = CoreExceptionCode.BadRequest;

    /// <summary>
    /// Gets a value indicating whether the result is a success result.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the result is a failure result.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the errors.
    /// </summary>
    public IEnumerable<ICoreException> Errors { get; }
        = Enumerable.Empty<ICoreException>();

    internal protected Result(bool isSuccess, IEnumerable<ICoreException> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public ICoreException? FirstErrorOrDefault()
        => Errors.FirstOrDefault();

    /// <summary>
    /// Ensure success result. If there is any error, it throws an <see cref="AggregateCoreException"/>
    /// </summary>
    /// <exception cref="AggregateCoreException"></exception>
    public void EnsureSuccess()
    {
        if (Errors.Any())
            throw new AggregateCoreException(Errors);
    }

    public static Result<TResult> Success<TResult>(TResult result)
        => Result<TResult>.Success(result);

    /// <summary>
    /// Failed result without value
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public static Result Failed(IEnumerable<ICoreException> errors)
        => new(
            false,
            errors?.ToList().AsReadOnly() ?? throw new ArgumentNullException("errors"));

    public static Result Success()
        => new Result(true, Enumerable.Empty<ICoreException>());

    internal record Error(string Message, CoreExceptionCode StatusCode) : ICoreException;
}

public class Result<TResult> : Result
{
    private TResult? _result;

    /// <summary>
    /// Get result, if there are errors, don't throw anything.
    /// </summary>
    public TResult? ResultValue => _result;

    /// <summary>
    /// Get result, if there are errors, throw an 'AggregateCoreException'.
    /// </summary>
    public TResult RequiredResult =>
        _result
        ?? throw new AggregateCoreException("Empty result.", Errors);

    protected internal Result(TResult? result, bool isSuccess, IEnumerable<ICoreException> errors) : base(isSuccess, errors)
    {
        _result = result;
    }

    public bool TryGetResult([NotNullWhen(true)] out TResult? result)
    {
        result = default;

        if (_result is null)
            return false;

        result = _result;
        return true;
    }

    /// <summary>
    /// Success result with value
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public static Result<TResult> Success(TResult result)
        => new(
            result,
            true,
            Enumerable.Empty<ICoreException>());

    /// <summary>
    /// Failed result without value
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public new static Result<TResult> Failed(IEnumerable<ICoreException> errors)
        => new(
            default,
            false,
            errors?.ToList().AsReadOnly() ?? throw new ArgumentNullException("errors"));

    /// <summary>
    /// Failed result without value
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public static Result<TResult> Failed(ICoreException error)
        => new(
            default,
            false,
            new ICoreException[] { error ?? throw new ArgumentNullException("error") });

    /// <summary>
    /// Failed result without value
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public static Result<TResult> Failed(string message, CoreExceptionCode statusCode = DEFAULT_STATUS_ERROR)
        => new(
            default,
            false,
            new ICoreException[] { new Error(message ?? throw new ArgumentNullException("message"), statusCode) });
}
