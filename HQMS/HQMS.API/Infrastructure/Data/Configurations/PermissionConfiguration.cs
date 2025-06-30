using HQMS.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HQMS.API.Infrastructure.Data.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");

            builder.HasOne(p => p.Menu)
                .WithMany(m => m.Permissions)
                .HasForeignKey(p => p.MenuId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
