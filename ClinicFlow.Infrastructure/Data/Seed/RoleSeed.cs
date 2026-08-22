using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Data.Seed
{
    public static class RoleSeed
    {

        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(


                  new Role
                  {
                      Id = 1,
                      Name = "SuperAdmin",
                      IsActive = true,
                      Permissions =
                    (int)(PermissionEnum.All)
                  },

                new Role
                {
                    Id = 2,
                    Name = "ClinicOwner",
                    IsActive = true,
                    Permissions =
                    (int)(
                          PermissionEnum.DoctorsView | PermissionEnum.DoctorsCreate | PermissionEnum.DoctorsUpdate |PermissionEnum.DoctorsDelete 
                        | PermissionEnum.DoctorsDelete | PermissionEnum.DoctorsDelete| PermissionEnum.DoctorsViewAll| PermissionEnum.PatientsView
                        | PermissionEnum.PatientsViewAll | PermissionEnum.PatientsCreate | PermissionEnum.PatientsUpdate | PermissionEnum.PatientsDelete
                        | PermissionEnum.ReceptionistsView | PermissionEnum.ReceptionistsViewAll | PermissionEnum.ReceptionistsCreate | PermissionEnum.ReceptionistsUpdate 
                        | PermissionEnum.ReceptionistsDelete| PermissionEnum.ClinicsSettings | PermissionEnum.ClinicsView | PermissionEnum.ClinicsUpdate
                        | PermissionEnum.ClinicsCreate | PermissionEnum.DoctorSchedulesView | PermissionEnum.DoctorSchedulesCreate | PermissionEnum.DoctorSchedulesUpdate 
                        | PermissionEnum.DoctorSchedulesDelete | PermissionEnum.DoctorVacationsView | PermissionEnum.DoctorVacationsViewAll
                        | PermissionEnum.DoctorVacationsCreate | PermissionEnum.DoctorVacationsUpdate | PermissionEnum.DoctorVacationsDelete

                    )
                },

                new Role
                {
                    Id = 3,
                    Name = "Doctor",
                    IsActive = true,
                    Permissions =
                    (int)(
                        PermissionEnum.DoctorsView | PermissionEnum.DoctorsUpdate | PermissionEnum.DoctorSchedulesView |
                        PermissionEnum.DoctorSchedulesUpdate | PermissionEnum.DoctorSchedulesDelete
                    )
                },

                new Role
                {
                    Id = 4,
                    Name = "Receptionist",
                    IsActive = true,
                    Permissions =
                    (int)(
                        PermissionEnum.DoctorsView | PermissionEnum.ReceptionistsView | PermissionEnum.ReceptionistsUpdate | PermissionEnum.PatientsView
                    )
                }
            );
        }


    }
}
