using HQMS.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HQMS.API.Infrastructure.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            // Table name
            builder.ToTable("Appointments");

            // Primary Key
            builder.HasKey(a => a.Id);

            // Required Properties
            builder.Property(a => a.DoctorId).IsRequired();
            builder.Property(a => a.PatientId).IsRequired();
            builder.Property(a => a.AppointmentTime).IsRequired();
            builder.Property(a => a.QueueGenerated)
                   .IsRequired()
                   .HasDefaultValue(false);

            // Relationships
            builder.HasOne(a => a.Patient)
                   .WithMany(p => p.Appointments)
                   .HasForeignKey(a => a.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Doctor)
                   .WithMany(d => d.Appointments)
                   .HasForeignKey(a => a.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(a => a.AppointmentTime);
            builder.HasIndex(a => a.QueueGenerated);
            builder.HasIndex(a => new { a.DoctorId, a.PatientId });
            builder.HasIndex(a => new { a.DoctorId, a.AppointmentTime });
        }
    }
}
