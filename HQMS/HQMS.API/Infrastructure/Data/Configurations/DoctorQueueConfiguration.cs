
using HQMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HQMS.API.Infrastructure.Data.Configurations
{
    public class DoctorQueueConfiguration : IEntityTypeConfiguration<DoctorQueue>
    {
        public void Configure(EntityTypeBuilder<DoctorQueue> builder)
        {
            builder.ToTable("DoctorQueues");

            builder.HasOne(dq => dq.Doctor)
                .WithMany(d => d.DoctorQueues)
                .HasForeignKey(dq => dq.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
