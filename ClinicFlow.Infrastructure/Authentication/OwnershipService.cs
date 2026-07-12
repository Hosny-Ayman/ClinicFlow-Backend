using ClinicFlow.Application.Common.Interfaces;

namespace ClinicFlow.Infrastructure.Authentication
{
    public class OwnershipService : IOwnershipService
    {

        private readonly ICurrentUserService _currentUserService;

        public OwnershipService(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public bool HasClinicAccess(int clinicId)
        {
            return _currentUserService.ClinicId == clinicId;
        }
    }
}
