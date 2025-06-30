using HQMS.API.Domain.Entities;
using HQMS.Application.Common;
using HQMS.Domain.Common;
using HQMS.Domain.Entities;
using HQMS.Domain.Entities.Common;
using HQMS.Domain.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HQMS.Infrastructure.Data
{
    public sealed class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IUserContextService _userContext;
        private readonly IDomainEventPublisher _eventPublisher;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IUserContextService userContext,
            IDomainEventPublisher eventPublisher)
            : base(options)
        {
            _userContext = userContext;
            _eventPublisher = eventPublisher;
        }
        // Dapper support
        public DbConnection GetConnection() => Database.GetDbConnection();

        // DbSets --------------------------------------------------------------
        public DbSet<DoctorQueue> DoctorQueues { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<QueueItem> QueueItems { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorSlot> DoctorSlots { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }

        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        // --------------------------------------------------------------------

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();

            // 1. Collect domain events BEFORE commit
            var domainEvents = ChangeTracker.Entries<BaseEntity>()
                                            .SelectMany(e => e.Entity.DomainEvents)
                                            .Where(e => e != null)
                                            .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            // 2. Clear events AFTER commit
            foreach (var entity in ChangeTracker.Entries<BaseEntity>())
            {
                entity.Entity.ClearDomainEvents();
            }

            // 3. Publish events
            foreach (var domainEvent in domainEvents)
            {
                await _eventPublisher.PublishAsync(domainEvent, cancellationToken);
            }

            return result;
        }

        // --------------------------------------------------------------------
        private void ApplyAuditInfo()
        {
            var user = _userContext.GetUserName() ?? "System";

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = user;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = user;
                }
            }
        }
    }
}