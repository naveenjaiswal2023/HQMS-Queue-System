
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Interfaces;
using HQMS.Domain.Interfaces;
using HQMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

public interface IUnitOfWork : IDisposable
{
    // Existing methods
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();

    // Repositories
    IAppointmentRepository AppointmentRepository { get; }
    IRepository<Patient> PatientRepository { get; }
    IRepository<Menu> MenuRepository { get; }
    IRoleMenuRepository RoleMenuRepository { get; }
    IRolePermissionRepository RolePermissionRepository { get; }
    IQueueRepository QueueRepository { get; }
    IDoctorQueueRepository DoctorQueueRepository { get; }
    IDoctorRepository DoctorRepository { get; }

    //IRepository<QueueEntry> QueuesEntries { get; }

    // ✅ Add this line
    ApplicationDbContext Context { get; }
}
