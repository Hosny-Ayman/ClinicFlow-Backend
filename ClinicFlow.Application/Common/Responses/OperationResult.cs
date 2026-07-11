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

        public static OperationResult<T> BadRequest(params Error[] errors)
        {
            return new(false, OperationStatus.BadRequest, default, errors.ToList());
        }

        public static OperationResult<T> NotFound(params Error[] errors)
        {
            return new(false, OperationStatus.NotFound, default, errors.ToList());
        }

        public static OperationResult<T> Conflict(params Error[] errors)
        {
            return new(false, OperationStatus.Conflict, default, errors.ToList());
        }

        public static OperationResult<T> Unauthorized(params Error[] errors)
        {
            return new(false, OperationStatus.Unauthorized, default, errors.ToList());
        }

        public static OperationResult<T> Forbidden(params Error[] errors)
        {
            return new(false, OperationStatus.Forbidden, default, errors.ToList());
        }

        public static OperationResult<T> Failure(params Error[] errors)
        {
            return new(false, OperationStatus.ServerError, default, errors.ToList());
        }

       

    }
}
