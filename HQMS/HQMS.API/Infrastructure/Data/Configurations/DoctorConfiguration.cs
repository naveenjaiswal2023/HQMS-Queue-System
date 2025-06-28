using HQMS.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HQMS.API.Infrastructure.Data.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            // Table name
            builder.ToTable("Doctors");

            // Primary key
            builder.HasKey(d => d.Id);

            // Required properties
            builder.Property(d => d.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.Email)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.Property(d => d.Specialization)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.Qualification)
                   .HasMaxLength(100);

            builder.Property(d => d.RegistrationNumber)
                   .HasMaxLength(50);

            builder.Property(d => d.ExperienceInYears)
                   .IsRequired();

            builder.Property(d => d.ConsultationType)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasDefaultValue("InPerson");

            builder.Property(d => d.IsAvailable)
                   .IsRequired()
                   .HasDefaultValue(true);

            // Foreign key: Hospital
            builder.Property(d => d.HospitalId)
                   .IsRequired();

            builder.HasOne(d => d.Hospital) // ✅ link navigation
           .WithMany(h => h.Doctors) // optional if Hospital has `public ICollection<Doctor> Doctors`
           .HasForeignKey(d => d.HospitalId)
           .OnDelete(DeleteBehavior.Restrict);


            // Foreign key: Department
            builder.Property(d => d.DepartmentId)
                   .IsRequired();

            builder.HasOne(d => d.Department)
                   .WithMany()
                   .HasForeignKey(d => d.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            // RowVersion (Concurrency)
            builder.Property(d => d.RowVersion)
                   .IsRowVersion();

            // Audit fields
            builder.Property(d => d.CreatedAt)
                   .IsRequired();

            builder.Property(d => d.CreatedBy)
                   .HasMaxLength(100);

            builder.Property(d => d.ModifiedBy)
                   .HasMaxLength(100);

            builder.Property(d => d.ModifiedAt);

            builder.Property(d => d.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(d => d.DeletedAt);

            builder.Property(d => d.DeletedBy)
                   .HasMaxLength(100);

            // Indexes for faster lookup
            builder.HasIndex(d => d.Email).IsUnique();
            builder.HasIndex(d => d.PhoneNumber);
        }
    }
}
