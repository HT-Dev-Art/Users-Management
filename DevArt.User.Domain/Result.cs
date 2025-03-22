namespace DevArt.Users.Core;

public struct Result<TValue>
{
    public Result(TValue value)
    {
        Value = value;
        IsSuccess = true;
        Exception = null;
    }

    public Result(Exception exception)
    {
        Value = default;
        IsSuccess = false;
        Exception = exception;
    }

    public TValue? Value { get; }

    public bool IsSuccess { get; }

    public Exception? Exception { get; }

    public static implicit operator Result<TValue>(TValue value)
    {
        return new Result<TValue>(value);
    }

    public static implicit operator Result<TValue>(Exception exception)
    {
        return new Result<TValue>(exception);
    }

    public TDestination ConvertTo<TDestination>(
        Func<TValue, TDestination> successConversionFn,
        Func<Exception, TDestination> failureConversionFn
    )
    {
        return IsSuccess ? successConversionFn(Value!) : failureConversionFn(Exception!);
    }

    public async Task<TDestination> ConvertTo<TDestination>(
        Func<TValue, Task<TDestination>> successConversionAsyncFn,
        Func<Exception, TDestination> failureConversionFn
    )
    {
        return IsSuccess ? await successConversionAsyncFn(Value!) : failureConversionFn(Exception!);
    }

    public async Task<TDestination> ConvertTo<TDestination>(
        Func<TValue, TDestination> successConversionFn,
        Func<Exception, Task<TDestination>> failureConversionAsyncFn
    )
    {
        return IsSuccess ? successConversionFn(Value!) : await failureConversionAsyncFn(Exception!);
    }

    public async Task<TDestination> ConvertTo<TDestination>(
        Func<TValue, Task<TDestination>> successConversionAsyncFn,
        Func<Exception, Task<TDestination>> failureConversionAsyncFn
    )
    {
        return IsSuccess ? await successConversionAsyncFn(Value!) : await failureConversionAsyncFn(Exception!);
    }
}