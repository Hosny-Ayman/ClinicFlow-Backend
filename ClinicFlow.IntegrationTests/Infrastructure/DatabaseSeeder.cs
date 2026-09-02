using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.IntegrationTests.Infrastructure
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // SAFETY GUARD
            if (!context.Database.GetDbConnection().ConnectionString.Contains("IntegrationTests"))
            {
                throw new Exception("SAFETY GUARD: Refusing to seed non-integration-test database.");
            }

            await SeedRolesAsync(context);
            await SeedSpecialtiesAsync(context);
            await SeedPaymentMethodsAsync(context);
            
            await SeedClinicsAsync(context);
            await SeedPersonsAndUsersAsync(context);
            await SeedDoctorsAsync(context);
            await SeedDoctorSchedulesAsync(context);
            await SeedDoctorVacationsAsync(context);
            await SeedPatientsAsync(context);
            await SeedAppointmentsAsync(context);

            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAsync(AppDbContext context)
        {
            if (!await context.Roles.AnyAsync(r => r.Id == 1))
            {
                context.Roles.AddRange(
                    new Role { Id = 1, Name = "SuperAdmin", IsActive = true, Permissions = -1, Description = "SuperAdmin" },
                    new Role { Id = 2, Name = "ClinicOwner", IsActive = true, Permissions = 4398045855743, Description = "ClinicOwner" },
                    new Role { Id = 3, Name = "Doctor", IsActive = true, Permissions = 4395926290473, Description = "Doctor" },
                    new Role { Id = 4, Name = "Receptionist", IsActive = true, Permissions = 892279465377, Description = "Receptionist" }
                );
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Roles ON");
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Roles OFF");
                    await transaction.CommitAsync();
                }
            }
        }

        private static async Task SeedSpecialtiesAsync(AppDbContext context)
        {
            if (!await context.Specialties.AnyAsync())
            {
                context.Specialties.Add(new Specialty { Id = 1, Name = "General Practice", IsActive = true });
                context.Specialties.Add(new Specialty { Id = 3, Name = "Cardiology", IsActive = true });
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Specialties ON");
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Specialties OFF");
                    await transaction.CommitAsync();
                }
            }
        }

        private static async Task SeedPaymentMethodsAsync(AppDbContext context)
        {
            if (!await context.PaymentMethods.AnyAsync())
            {
                context.PaymentMethods.AddRange(
                    new PaymentMethod { Id = 1, Name = "Cash", IsActive = true },
                    new PaymentMethod { Id = 2, Name = "Visa", IsActive = true }
                );
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT PaymentMethods ON");
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT PaymentMethods OFF");
                    await transaction.CommitAsync();
                }
            }
        }

        private static async Task SeedClinicsAsync(AppDbContext context)
        {
            if (!await context.Clinics.AnyAsync(c => c.Id == 1))
            {
                var clinicA = new Clinic { Id = 1, Name = "Clinic A", Phone = "111111111", Email = "clinica@g", Address = "Address A", IsActive = true, CreatedAt = DateTime.UtcNow };
                var clinicB = new Clinic { Id = 2, Name = "Clinic B", Phone = "222222222", Email = "clinicb@g", Address = "Address B", IsActive = true, CreatedAt = DateTime.UtcNow };
                
                context.Clinics.AddRange(clinicA, clinicB);
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Clinics ON");
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Clinics OFF");
                    await transaction.CommitAsync();
                }
                
                // Working hours
                for (int i = 0; i <= 6; i++)
                {
                    context.ClinicWorkingHours.Add(new ClinicWorkingHour { ClinicId = 1, Day = (DayOfWeek)i, OpenTime = new TimeOnly(9, 0, 0), CloseTime = new TimeOnly(17, 0, 0), IsClosed = false, AppointmentDurationInMinutes = 15 });
                    context.ClinicWorkingHours.Add(new ClinicWorkingHour { ClinicId = 2, Day = (DayOfWeek)i, OpenTime = new TimeOnly(9, 0, 0), CloseTime = new TimeOnly(17, 0, 0), IsClosed = false, AppointmentDurationInMinutes = 15 });
                }
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedPersonsAndUsersAsync(AppDbContext context)
        {
            if (!await context.Users.AnyAsync(u => u.Id == 1009)) // SuperAdmin
            {
                // This assumes standard PasswordHash for "stringstring"
                var passwordHash = "$2a$11$7O5dVwMs1qvvJN.VS5SMXekZfpRT2luneirnHi4GCSny62KDeaLnG"; 
                var passwordHashChangeMe = passwordHash; // Replaced by real hashes if needed

                // SuperAdmin
                var personSA = new Person { Id = 1009, FirstName = "Super", LastName = "Admin", Email = TestCredentials.SuperAdminEmail };
                var userSA = new User { Id = 1009, PersonId = 1009, ClinicId = 1, PasswordHash = passwordHash, IsActive = true, CreatedAt = DateTime.UtcNow };
                
                // ClinicOwner A
                var personOwnerA = new Person { Id = 1, FirstName = "Owner", LastName = "A", Email = TestCredentials.ClinicAOwnerEmail };
                var userOwnerA = new User { Id = 1, PersonId = 1, ClinicId = 1, PasswordHash = passwordHash, IsActive = true, CreatedAt = DateTime.UtcNow };
                
                // Doctor A
                var personDocA = new Person { Id = 8, FirstName = "Doctor", LastName = "A", Email = TestCredentials.ClinicADoctor1Email };
                var userDocA = new User { Id = 8, PersonId = 8, ClinicId = 1, PasswordHash = passwordHashChangeMe, IsActive = true, CreatedAt = DateTime.UtcNow };

                // Receptionist A
                var personRecA = new Person { Id = 4, FirstName = "Receptionist", LastName = "A", Email = TestCredentials.ClinicAReceptionistEmail };
                var userRecA = new User { Id = 4, PersonId = 4, ClinicId = 1, PasswordHash = passwordHashChangeMe, IsActive = true, CreatedAt = DateTime.UtcNow };

                // ClinicOwner B
                var personOwnerB = new Person { Id = 5, FirstName = "Owner", LastName = "B", Email = TestCredentials.ClinicBOwnerEmail };
                var userOwnerB = new User { Id = 5, PersonId = 5, ClinicId = 2, PasswordHash = passwordHashChangeMe, IsActive = true, CreatedAt = DateTime.UtcNow };

                context.Persons.AddRange(personSA, personOwnerA, personDocA, personRecA, personOwnerB);
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Persons ON");
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Persons OFF");
                    await transaction.CommitAsync();
                }

                context.Users.AddRange(userSA, userOwnerA, userDocA, userRecA, userOwnerB);
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Users ON");
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Users OFF");
                    await transaction.CommitAsync();
                }

                // Roles Mapping
                context.UserRoles.AddRange(
                    new UserRole { UserId = 1009, RoleId = 1 }, // SuperAdmin
                    new UserRole { UserId = 1, RoleId = 2 }, // Owner A
                    new UserRole { UserId = 1, RoleId = 3 }, // Doctor A (Owner A is also a Doctor in real data)
                    new UserRole { UserId = 8, RoleId = 3 }, // Doctor A
                    new UserRole { UserId = 4, RoleId = 4 }, // Receptionist A
                    new UserRole { UserId = 5, RoleId = 2 }  // Owner B
                );
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedDoctorsAsync(AppDbContext context)
        {
            if (!await context.Doctors.AnyAsync(d => d.UserId == 8))
            {
                context.Doctors.AddRange(
                    new Doctor { Id = 1, UserId = 1, ClinicId = 1, SpecialtyId = 3, ConsultationFee = 300, Gender = GenderEnum.Male, ExperienceYears = 5 },
                    new Doctor { Id = 2, UserId = 8, ClinicId = 1, SpecialtyId = 1, ConsultationFee = 200, Gender = GenderEnum.Male, ExperienceYears = 2 }
                );
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Doctors ON");
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Doctors OFF");
                    await transaction.CommitAsync();
                }
            }
        }

                private static async Task SeedDoctorSchedulesAsync(AppDbContext context)
        {
            if (!await context.DoctorSchedules.AnyAsync(ds => ds.DoctorId == 2))
            {
                var schedules = new List<ClinicFlow.Domain.Entities.DoctorSchedule>();
                int idCounter = 1;
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    schedules.Add(new ClinicFlow.Domain.Entities.DoctorSchedule
                    {
                        Id = idCounter++,
                        DoctorId = 1, // Owner A
                        DayOfWeek = day,
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(17, 0),
                        IsAvailable = day != DayOfWeek.Sunday && day != DayOfWeek.Friday
                    });
                    
                    schedules.Add(new ClinicFlow.Domain.Entities.DoctorSchedule
                    {
                        Id = idCounter++,
                        DoctorId = 2, // Doctor A
                        DayOfWeek = day,
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(17, 0),
                        IsAvailable = day != DayOfWeek.Sunday && day != DayOfWeek.Friday
                    });
                }
                
                context.DoctorSchedules.AddRange(schedules);
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT DoctorSchedules ON");
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT DoctorSchedules OFF");
                    await transaction.CommitAsync();
                }
            }
        }

                private static async Task SeedDoctorVacationsAsync(AppDbContext context)
        {
            if (!await context.DoctorVacations.AnyAsync(dv => dv.Id == 1))
            {
                context.DoctorVacations.AddRange(
                    new ClinicFlow.Domain.Entities.DoctorVacation
                    {
                        Id = 1,
                        DoctorId = 2, // Doctor A (UserId=8, ClinicId=1)
                        StartDate = new DateOnly(2026, 1, 1),
                        EndDate = new DateOnly(2026, 1, 10),
                        Reason = "Test Vacation A",
                        Status = ClinicFlow.Domain.Enums.DoctorVacationStatusEnum.Completed
                    },
                    new ClinicFlow.Domain.Entities.DoctorVacation
                    {
                        Id = 2,
                        DoctorId = 1, // Owner A (UserId=1, ClinicId=1)
                        StartDate = new DateOnly(2026, 2, 1),
                        EndDate = new DateOnly(2026, 2, 10),
                        Reason = "Test Vacation B",
                        Status = ClinicFlow.Domain.Enums.DoctorVacationStatusEnum.Completed
                    }
                );
                
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT DoctorVacations ON");
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT DoctorVacations OFF");
                    await transaction.CommitAsync();
                }
            }
        }

                private static async Task SeedAppointmentsAsync(AppDbContext context)
        {
            if (!await context.Appointments.AnyAsync(a => a.Id == 1))
            {
                context.Appointments.AddRange(
                    new ClinicFlow.Domain.Entities.Appointment
                    {
                        Id = 1,
                        PatientId = 1,
                        DoctorId = 2,
                        ClinicId = 1,
                        AppointmentDate = new DateOnly(2030, 1, 7), // Monday
                        StartTime = new TimeOnly(11, 0),
                        EndTime = new TimeOnly(11, 30),
                        Status = ClinicFlow.Domain.Enums.AppointmentStatusEnum.Scheduled,
                        Notes = "Seeded Appointment",
                        CreatedAt = DateTime.UtcNow
                    }
                );
                
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Appointments ON");
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Appointments OFF");
                    await transaction.CommitAsync();
                }
            }
        }

        private static async Task SeedPatientsAsync(AppDbContext context)
        {
            if (!await context.Patients.AnyAsync(p => p.Id == 1))
            {
                var personPatA = new Person { Id = 2, FirstName = "Patient", LastName = "A", Email = "patienta@g" };
                var personPatB = new Person { Id = 6, FirstName = "Patient", LastName = "B", Email = "patientb@g" };
                
                context.Persons.AddRange(personPatA, personPatB);
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Persons ON");
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Persons OFF");
                    await transaction.CommitAsync();
                }

                context.Patients.AddRange(
                    new Patient { Id = 1, PersonId = 2, DateOfBirth = new DateOnly(1990, 1, 1), Gender = GenderEnum.Male },
                    new Patient { Id = 2, PersonId = 6, DateOfBirth = new DateOnly(1995, 1, 1), Gender = GenderEnum.Female }
                );
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Patients ON");
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Patients OFF");
                    await transaction.CommitAsync();
                }

                context.ClinicPatients.AddRange(
                    new ClinicPatient { Id = 1, ClinicId = 1, PatientId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new ClinicPatient { Id = 2, ClinicId = 2, PatientId = 2, IsActive = true, CreatedAt = DateTime.UtcNow }
                );
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT ClinicPatients ON");
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT ClinicPatients OFF");
                    await transaction.CommitAsync();
                }
            }
        }
    }
}









