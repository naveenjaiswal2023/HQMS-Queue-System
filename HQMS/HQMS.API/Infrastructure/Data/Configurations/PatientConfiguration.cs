using HQMS.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HQMS.Infrastructure.Data.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            // Table Name
            builder.ToTable("Patients");

            // Primary Key
            builder.HasKey(p => p.PatientId);

            // Scalar Properties
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Age)
                   .IsRequired();

            builder.Property(p => p.Gender)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(p => p.PhoneNumber)
                   .HasMaxLength(15);

            builder.Property(p => p.Email)
                   .HasMaxLength(100);

            builder.Property(p => p.Address)
                   .HasMaxLength(200);

            builder.Property(p => p.BloodGroup)
                   .HasMaxLength(10);

            builder.Property(p => p.CreatedAt)
                   .IsRequired();

            builder.Property(p => p.ModifiedAt);
            builder.Property(p => p.CreatedBy).HasMaxLength(100);
            builder.Property(p => p.ModifiedBy).HasMaxLength(100);
            builder.Property(p => p.DeletedBy).HasMaxLength(100);
            builder.Property(p => p.DeletedAt);
            builder.Property(p => p.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(p => p.RowVersion)
                   .IsRowVersion();

            // Foreign Key Properties
            builder.Property(p => p.DepartmentId)
                   .IsRequired();

            builder.Property(p => p.HospitalId)
                   .IsRequired();

            builder.Property(p => p.PrimaryDoctorId)
                   .IsRequired();

            // Relationships
            // Relationships
            builder.HasOne(p => p.Department)
                   .WithMany()
                   .HasForeignKey(p => p.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Hospital)
                   .WithMany(h => h.Patients)
                   .HasForeignKey(p => p.HospitalId)
                   .HasPrincipalKey(h => h.HospitalId) // 👈 Explicitly map FK to PK
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.PrimaryDoctor)
                   .WithMany()
                   .HasForeignKey(p => p.PrimaryDoctorId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Indexes
            builder.HasIndex(p => p.PhoneNumber);
            builder.HasIndex(p => p.Email);
            builder.HasIndex(p => new { p.HospitalId, p.DepartmentId });

            // Ignore domain events
            builder.Ignore(p => p.DomainEvents);
        }
    }
}