namespace ClinicFlow.Domain.Entities
{
    public class Attachment
    {
        public int Id { get; set; }

        public int MedicalRecordId { get; set; }

        public string FileName { get; set; } = null!;

        public string FileUrl { get; set; } = null!;

        public string ContentType { get; set; } = null!;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public MedicalRecord MedicalRecord { get; set; } = null!;
    }
















}
