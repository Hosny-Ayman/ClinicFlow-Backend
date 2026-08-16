namespace ClinicFlow.Domain.Interfaces
{
    public interface IUnitOfWork
    {

        Task<int> SaveChangesAsync();

    }
}
