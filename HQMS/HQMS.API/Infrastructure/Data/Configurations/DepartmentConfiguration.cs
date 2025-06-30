using HQMS.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HQMS.API.Infrastructure.Data.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            // Table Name
            builder.ToTable("Departments");

            // Primary Key
            builder.HasKey(d => d.DepartmentId);

            // Property Configurations
            builder.Property(d => d.DepartmentId)
                   .IsRequired();

            builder.Property(d => d.DepartmentName)
                   .IsRequired()
                   .HasMaxLength(100);

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

            builder.Property(d => d.RowVersion)
                   .IsRowVersion();

            // Index for fast lookup by name
            builder.HasIndex(d => d.DepartmentName)
                   .HasDatabaseName("IX_Departments_DepartmentName");
            // Foreign key: HospitalId
            builder.HasOne(d => d.Hospital)
                   .WithMany(h => h.Departments)
                   .HasForeignKey(d => d.HospitalId)
                   .OnDelete(DeleteBehavior.Restrict); // Or Cascade/SetNull based on your needs

        }
    }
}
