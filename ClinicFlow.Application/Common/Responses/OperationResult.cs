using ClinicFlow.Application.Common.Errors;


namespace ClinicFlow.Application.Common.Responses
{
    public class OperationResult<T>
    {
        public bool IsSuccess { get; }

        public OperationStatus Status { get; }

        public List <Error>? Errors { get; }

        public T? Data { get; }

        private OperationResult( bool isSuccess,OperationStatus status,T? data,List <Error>? errors)
        {
            IsSuccess = isSuccess;
            Status = status;
            Data = data;
            Errors = errors;
        }

        public static OperationResult<T> Success(T data)
        {
            return new(true, OperationStatus.Success, data, null);
        }

        public static OperationResult<T> BadRequest(List <Error> error)
        {
            return new(false, OperationStatus.BadRequest, default, error);
        }

        public static OperationResult<T> NotFound(List<Error> error)
        {
            return new(false, OperationStatus.NotFound, default, error);
        }

        public static OperationResult<T> Conflict(List<Error> error)
        {
            return new(false, OperationStatus.Conflict, default, error);
        }

        public static OperationResult<T> Unauthorized(List<Error> error)
        {
            return new(false, OperationStatus.Unauthorized, default, error);
        }

        public static OperationResult<T> Forbidden(List<Error> error)
        {
            return new(false, OperationStatus.Forbidden, default, error);
        }

        public static OperationResult<T> Failure(List<Error> error)
        {
            return new(false, OperationStatus.ServerError, default, error);
        }

       

    }
}
