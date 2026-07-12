namespace ClinicFlow.Application.Common.Interfaces
{
    public interface IOwnershipService
    {

        bool HasClinicAccess(int clinicId);
    }
}
