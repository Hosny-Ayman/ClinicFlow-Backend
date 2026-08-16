using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.Interfaces
{
    public interface ISpecialtyRepository
    {

        Task<Specialty?> getSpecialtyById(int SpecialtyId);

        Task<List<Specialty>> getAllSpecialtiesAsync();


    }
}
