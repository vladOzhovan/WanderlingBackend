namespace Wanderling.Application.Results
{
    public class Result<T>
    {
        public bool Succeeded { get; init; }
        public T? Data { get; init; }
        public IReadOnlyCollection<Error> Errors { get; init; } = Array.Empty<Error>();

        public static Result<T> Ok(T data) => new() { Succeeded = true, Data = data };
        public static Result<T> Fail(params Error[] errors) => new() { Succeeded = false, Errors = errors };
    }
}
