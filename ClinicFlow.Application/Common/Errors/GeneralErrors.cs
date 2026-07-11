namespace ClinicFlow.Application.Common.Errors
{
    public class GeneralErrors
    {

        public static Error Validation(string message) => new("ValidationError", message);

        public static Error NotFound(string name) => new("NotFound", $"{name} not found");

        public static Error Conflict(string message) => new("Conflict", message);

        public static Error Unauthorized(string message) => new("Unauthorized", message);
    }
}
