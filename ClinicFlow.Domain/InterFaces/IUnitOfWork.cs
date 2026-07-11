namespace ClinicFlow.Domain.InterFaces
{
    public interface IUnitOfWork
    {

        Task<int> SaveChangesAsync();

    }
}
