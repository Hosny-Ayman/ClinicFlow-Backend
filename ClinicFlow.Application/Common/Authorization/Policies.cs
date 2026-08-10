namespace ClinicFlow.Application.Common.Authorization
{
    public static class Policies
    {


        public const string DoctorsView = nameof(DoctorsView);
        public const string DoctorsViewAll = nameof(DoctorsViewAll);
        public const string DoctorsCreate = nameof(DoctorsCreate);
        public const string DoctorsUpdate = nameof(DoctorsUpdate);
        public const string DoctorsDelete = nameof(DoctorsDelete);


        public const string PatientsView = nameof(PatientsView);
        public const string PatientsCreate = nameof(PatientsCreate);
        public const string PatientsUpdate = nameof(PatientsUpdate);
        public const string PatientsDelete = nameof(PatientsDelete);


        public const string ReceptionistsView = nameof(ReceptionistsView);
        public const string ReceptionistsCreate = nameof(ReceptionistsCreate);
        public const string ReceptionistsUpdate = nameof(ReceptionistsUpdate);
        public const string ReceptionistsDelete = nameof(ReceptionistsDelete);


        public const string ClinicSettings = nameof(ClinicSettings);

    }
}
