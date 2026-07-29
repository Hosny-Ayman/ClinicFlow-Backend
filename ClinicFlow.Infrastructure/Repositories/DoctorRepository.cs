using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.InterFaces;
using ClinicFlow.Infrastructure.Data;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {

        private readonly AppDbContext _appDbContext;


        public DoctorRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<int> AddDoctorAsync(Doctor doctor)
        {
           await _appDbContext.Doctors.AddAsync(doctor);

            return doctor.Id;
        }
    }
}
