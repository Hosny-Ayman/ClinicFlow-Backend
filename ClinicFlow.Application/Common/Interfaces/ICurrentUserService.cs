namespace ClinicFlow.Application.Common.Interfaces
{

        public interface ICurrentUserService
        {
            bool IsAuthenticated { get; }

            int? UserId { get; }

            string? Email { get; }

            int? ClinicId { get; }

            IReadOnlyList<string> Roles { get; }
    }

}
