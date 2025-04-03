
using TextProcessor.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TextProcessor.Helpers
{
    public sealed record class OperationResult(bool IsSuccess, string Error);
    public sealed record class OperationResult<TValue>(bool IsSuccess, TValue Value, string Error);
    public class OperationResultCreator
    {
        public static OperationResult Success => new OperationResult(true, string.Empty);
        public static OperationResult Failure(string error) => new OperationResult(false, error);
        public static OperationResult<T> SuccessWithValue<T>(T value) => new OperationResult<T>(true, value, string.Empty);
        public static OperationResult<T> Failure<T>(string error) => new OperationResult<T>(false, default!, error);
        public static OperationResult<T> MayBeNotFound<T>(T? result) => result switch
        {
            null => Failure<T>("not found"),
            not null => SuccessWithValue(result)
        };
    }
}
