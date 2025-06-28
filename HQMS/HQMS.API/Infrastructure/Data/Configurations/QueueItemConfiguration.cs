using HQMS.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HQMS.API.Infrastructure.Data.Configurations
{
    public class QueueItemConfiguration : IEntityTypeConfiguration<QueueItem>
    {
        public void Configure(EntityTypeBuilder<QueueItem> builder)
        {
            builder.ToTable("QueueItems");

            // Primary Key
            builder.HasKey(q => q.Id);

            // Required Properties
            builder.Property(q => q.PatientId)
                   .IsRequired();

            builder.Property(q => q.DoctorId)
                   .IsRequired();

            builder.Property(q => q.AppointmentId)
                   .IsRequired();

            builder.Property(q => q.Status)
                   .HasConversion<int>() // Assuming Status is an enum
                   .IsRequired();

            builder.Property(q => q.Position)
                   .IsRequired();

            builder.Property(q => q.EstimatedWaitTime)
                   .IsRequired();

            builder.Property(q => q.QueueNumber)
                   .HasMaxLength(20)
                   .IsRequired();

            // Optional: Audit properties (if inherited from BaseEntity)
            builder.Property(q => q.CreatedAt)
                   .IsRequired();

            // Foreign Key Relationships (if navigations exist)
            builder.HasOne(q => q.Patient)
                   .WithMany()
                   .HasForeignKey(q => q.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(q => q.Doctor)
                   .WithMany()
                   .HasForeignKey(q => q.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(q => q.Appointment)
                   .WithMany()
                   .HasForeignKey(q => q.AppointmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes for query performance
            builder.HasIndex(q => new { q.Status, q.Position })
                   .HasDatabaseName("IX_QueueItems_Status_Position");

            builder.HasIndex(q => new { q.PatientId, q.Status })
                   .HasDatabaseName("IX_QueueItems_PatientId_Status");

            builder.HasIndex(q => new { q.DoctorId, q.Status, q.Position })
                   .HasDatabaseName("IX_QueueItems_DoctorId_Status_Position");

            // Ignore Domain Events (EF Core doesn’t persist this)
            builder.Ignore(q => q.DomainEvents);
        }
    }
}
