using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.ToTable("Attachments");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.FileSize)
                .IsRequired();

            builder.Property(a => a.UploadedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(a => a.MedicalRecordId);

            builder.HasIndex(a => a.FileName);

            builder.HasIndex(a => a.ContentType);

            builder.HasIndex(a => a.UploadedAt);

            builder.HasIndex(a => new
            {
                a.MedicalRecordId,
                a.UploadedAt
            });

            builder.HasOne(a => a.MedicalRecord)
                .WithMany(m => m.Attachments)
                .HasForeignKey(a => a.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }



   













}
