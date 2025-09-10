using System.Diagnostics.CodeAnalysis;

namespace Bl.Mottu.Maintenance.Core.Primitive
{
    /// <summary>
    /// Manages a creation of <see cref="Result"/>
    /// </summary>
    public class ResultBuilder
    {
        protected internal List<ICoreException> _errors = new();
        public IEnumerable<ICoreException> Errors => _errors;
        public bool HasError => _errors.Any();

        public ResultBuilder()
        {
        }

        public ResultBuilder(Result result)
            : this(result.Errors)
        {
        }

        public ResultBuilder(IEnumerable<ICoreException> errors)
        {
            _errors.AddRange(errors);
        }

        public override string ToString()
            => string.Concat("Errors: ", _errors.Count);

        /// <summary>
        /// Add error to result if enumerable is empty
        /// </summary>
        public bool AddIfIsEmpty(System.Collections.IEnumerable toCheck, string message, CoreExceptionCode code = Result.DEFAULT_STATUS_ERROR)
        {
            foreach (var _ in toCheck)
            {
                _errors.Add(new Result.Error(message, code));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add error to result if string is null or white space
        /// </summary>
        public bool AddIfIsNullOrWhiteSpace(string? toCheck, string message, CoreExceptionCode code = Result.DEFAULT_STATUS_ERROR)
            => AddIfInternal(string.IsNullOrWhiteSpace(toCheck), message, code);

        /// <summary>
        /// Add error to result if string is null or empty
        /// </summary>
        public bool AddIfIsNullOrEmpty(string? toCheck, string message, CoreExceptionCode code = Result.DEFAULT_STATUS_ERROR)
            => AddIfInternal(string.IsNullOrEmpty(toCheck), message, code);

        /// <summary>
        /// Add error to result if object is null
        /// </summary>
        public bool AddIfIsNull(object? toCheck, string message, CoreExceptionCode code = Result.DEFAULT_STATUS_ERROR)
            => AddIfInternal(toCheck is null, message, code);

        /// <summary>
        /// Add error to result if condition is true
        /// </summary>
        public void AddIf(bool condition, CoreExceptionCode code)
        {
            if (condition)
                _errors.Add(new Result.Error(code.ToString(), code));
        }

        /// <summary>
        /// Add error to result if condition is true
        /// </summary>
        public void AddIf(bool condition, string message, CoreExceptionCode code = Result.DEFAULT_STATUS_ERROR)
        {
            if (condition)
                _errors.Add(new Result.Error(message, code));
        }

        private bool AddIfInternal(bool condition, string message, CoreExceptionCode code)
        {
            if (condition)
                _errors.Add(new Result.Error(message, code));
            return condition;
        }

        /// <summary>
        /// Add error to result
        /// </summary>
        public void Add(string error, CoreExceptionCode code = Result.DEFAULT_STATUS_ERROR)
                => _errors.Add(new Result.Error(error, code));

        /// <summary>
        /// Add range of errors to result
        /// </summary>
        public void AddRange(params string[] errors)
            => _errors.AddRange(errors.Select(e => new Result.Error(e, Result.DEFAULT_STATUS_ERROR)));

        /// <summary>
        /// Add range of errors to result
        /// </summary>
        public void AddRange(IEnumerable<string> errors)
            => _errors.AddRange(errors.Select(e => new Result.Error(e, Result.DEFAULT_STATUS_ERROR)));

        /// <summary>
        /// Add range of errors to result
        /// </summary>
        public void AddRange(params ICoreException[] errors)
            => _errors.AddRange(errors);

        /// <summary>
        /// Add range of errors to result
        /// </summary>
        public void AddRange(IEnumerable<ICoreException> errors)
            => _errors.AddRange(errors);

        /// <summary>
        /// Add range of errors to result
        /// </summary>
        internal void Add(Result.Error error)
            => _errors.Add(error);

        /// <summary>
        /// Add range of errors to result
        /// </summary>
        internal void AddRange(params Result.Error[] errors)
            => _errors.AddRange(errors);

        /// <summary>
        /// Add range of errors to result
        /// </summary>
        internal void AddRange(IEnumerable<Result.Error> errors)
            => _errors.AddRange(errors);

        /// <summary>
        /// Try get failed result
        /// </summary>
        public bool TryFailed([NotNullWhen(true)] out Result? resultT)
        {
            if (HasError)
            {
                resultT = Failed();
                return true;
            }

            resultT = null;
            return false;
        }

        /// <summary>
        /// Failed result, needs errors
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public Result Failed()
        {
            if (!HasError)
                throw new InvalidOperationException("There aren't errors registered.");

            return Result.Failed(_errors.AsReadOnly());
        }

        /// <summary>
        /// Success result, needs return obj
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public Result Success()
            => Result.Success(true);

        /// <summary>
        /// Create a failed or successful result.
        /// </summary>
        /// <param name="ifSuccess">Function that the result will be contained if success.</param>
        public Result CreateResult(Action ifSuccess)
        {
            if (_errors.Any())
            {
                return Result.Failed(_errors);
            }

            try
            {
                ifSuccess();
            }
            catch
            {
                return Result.Failed(
                    new[] { new CommonCoreException("Failed to get result.") });
            }

            return Result.Success();
        }
    }


    /// <summary>
    /// Manages a creation of <see cref="Result{TResult}"/>
    /// </summary>
    public sealed class ResultBuilder<TResult> : ResultBuilder
    {
        public ResultBuilder()
        {
        }

        public ResultBuilder(Result result)
            : base(result.Errors)
        {
        }

        public ResultBuilder(ResultBuilder<TResult> resultBuilder)
            : base(resultBuilder._errors)
        {
        }

        public ResultBuilder(IEnumerable<ICoreException> errors)
        {
            _errors.AddRange(errors);
        }

        /// <summary>
        /// Try get failed result
        /// </summary>
        public bool TryFailed([NotNullWhen(true)] out Result<TResult>? resultT)
        {
            if (HasError)
            {
                resultT = Failed();
                return true;
            }

            resultT = null;
            return false;
        }

        /// <summary>
        /// Failed result, needs errors
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public new Result<TResult> Failed()
        {
            if (!HasError)
                throw new InvalidOperationException("There aren't errors registered.");

            return Result<TResult>.Failed(_errors.AsReadOnly());
        }

        /// <summary>
        /// Success result, needs return obj
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public Result<TResult> Success(TResult result)
            => Result<TResult>.Success(result);

        /// <summary>
        /// Create a failed or successful result.
        /// </summary>
        /// <typeparam name="TResult">The returned type</typeparam>
        /// <param name="ifSuccess">Function that the result will be contained if success.</param>
        public Result<TResult> CreateResult(Func<TResult> ifSuccess)
        {
            if (_errors.Any())
            {
                return Result<TResult>.Failed(_errors);
            }

            TResult? validResult;
            try
            {
                validResult = ifSuccess();
            }
            catch
            {
                return Result<TResult>.Failed(new CommonCoreException("Failed to get result."));
            }

            return Result<TResult>.Success(validResult);
        }
    }
}
