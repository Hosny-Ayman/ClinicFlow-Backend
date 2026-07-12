using ClinicFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ClinicFlow.Domain.InterFaces
{
    public interface IClinicRepository
    {

        Task<Clinic?> GetClinicByIdAsync(int clinicId,bool Tracking = false);

        Task<Clinic?> GetClinicByNameAsync(string clinicName, bool Tracking = false);

        Task<int> AddAsync(Clinic clinic);

        Task UpdateAsync();

        Task<bool> IsClinicExistsByIdAsync(int clinicId);


    }
}
