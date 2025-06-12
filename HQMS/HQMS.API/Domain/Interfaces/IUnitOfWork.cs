using HospitalQueueSystem.Domain.Entities;
using HospitalQueueSystem.Infrastructure.Data;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Interfaces;
using HQMS.API.Infrastructure.Repositories;
using System.Data;
using System.Threading.Tasks;
namespace HospitalQueueSystem.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ApplicationDbContext Context { get; }
        IRepository<Patient> PatientRepository { get; }
        IQueueRepository QueueRepository { get; }
        IRepository<DoctorQueue> Queues { get; }
        IRepository<Menu> MenuRepository { get; }
        IRoleMenuRepository RoleMenuRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }

}
