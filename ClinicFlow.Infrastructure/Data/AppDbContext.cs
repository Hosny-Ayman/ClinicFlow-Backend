using ClinicFlow.Domain.Entities;
using ClinicFlow.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ClinicFlow.Infrastructure.Data
{
    public class AppDbContext: DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }



        public DbSet<Clinic> Clinics => Set<Clinic>();

        public DbSet<Person> Persons => Set<Person>();

        public DbSet<User> Users => Set<User>();

        public DbSet<Role> Roles => Set<Role>();

        public DbSet<Doctor> Doctors => Set<Doctor>();

        public DbSet<Specialty> Specialties => Set<Specialty>();

        public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();

        public DbSet<DoctorVacation> DoctorVacations => Set<DoctorVacation>();

        public DbSet<Patient> Patients => Set<Patient>();

        public DbSet<ClinicPatient> ClinicPatients => Set<ClinicPatient>();

        public DbSet<Appointment> Appointments => Set<Appointment>();

        public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();

        public DbSet<Prescription> Prescriptions => Set<Prescription>();

        public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();

        public DbSet<Invoice> Invoices => Set<Invoice>();

        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();

        public DbSet<Attachment> Attachments => Set<Attachment>();

        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public DbSet<UserRole> UserRoles => Set<UserRole>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<SysteamSetting> SysteamSettings => Set<SysteamSetting>();

        public DbSet<ClinicSetup> ClinicSetups => Set<ClinicSetup>();

        public DbSet<ClinicWorkingHour> ClinicWorkingHours => Set<ClinicWorkingHour>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            RoleSeed.Seed(modelBuilder);

        }

    }

   
}
