namespace ClinicFlow.Domain.Enums
{
    [Flags]
    public enum PermissionEnum:long
    {
        None = 0,

       
        DoctorsView = 1L << 0,
        DoctorsViewAll =1L <<1,
        DoctorsCreate = 1L << 2,
        DoctorsUpdate = 1L << 3,
        DoctorsDelete = 1L << 4,


        PatientsView = 1L  << 5,
        PatientsViewAll = 1L << 6,
        PatientsCreate = 1L << 7,
        PatientsUpdate = 1L << 8,
        PatientsDelete = 1L << 9,


        ReceptionistsView = 1L << 10,
        ReceptionistsViewAll = 1L << 11,
        ReceptionistsCreate = 1L << 12,
        ReceptionistsUpdate = 1L << 13,
        ReceptionistsDelete = 1L << 14,

        ClinicsCreate = 1L << 15,
        ClinicsView = 1L << 16,
        ClinicsViewAll = 1L << 17,
        ClinicsUpdate = 1L << 18,
        ClinicsDelete = 1L << 19,
        ClinicsSettings = 1L << 20,

        DoctorSchedulesView = 1 << 21,
        DoctorSchedulesCreate = 1 << 22,
        DoctorSchedulesUpdate = 1 << 23,
        DoctorSchedulesDelete = 1 << 24,


        All = -1
    }
}
