using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Interfaces;
using HQMS.API.Infrastructure.Repositories;
using HQMS.Domain.Entities;
using HQMS.Domain.Interfaces;
using HQMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HQMS.Infrastructure.Data
{
    public sealed class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction? _transaction;

        // Lazy repositories ---------------------------------------------------
        private Lazy<IDoctorRepository> _doctorRepo;
        private Lazy<IAppointmentRepository> _appointmentRepo;
        private Lazy<IRepository<Patient>> _patientRepo;
        private Lazy<IRepository<Menu>> _menuRepo;
        private Lazy<IRoleMenuRepository> _roleMenuRepo;
        private Lazy<IRolePermissionRepository> _rolePermissionRepo;
        private Lazy<IQueueRepository> _queueRepo;
        private Lazy<IDoctorQueueRepository> _doctorQueueRepo;
        // private Lazy<IRepository<QueueEntry>>   _queueEntryRepo;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            _doctorRepo = new(() => new DoctorRepository(_dbContext));
            _appointmentRepo = new(() => new AppointmentRepository(_dbContext));
            _patientRepo = new(() => new PatientRepository(_dbContext));
            _menuRepo = new(() => new MenuRepository(_dbContext));
            _roleMenuRepo = new(() => new RoleMenuRepository(_dbContext));
            _rolePermissionRepo = new(() => new RolePermissionRepository(_dbContext));
            _queueRepo = new(() => new QueueRepository(_dbContext));
            _doctorQueueRepo = new(() => new DoctorQueueRepository(_dbContext));
            //_queueEntryRepo      = new(() => new GenericRepository<QueueEntry>(_dbContext));
        }

        // --------------------------------------------------------------------
        public ApplicationDbContext Context => _dbContext;

        // Repository accessors -----------------------------------------------
        public IDoctorRepository DoctorRepository => _doctorRepo.Value;
        public IAppointmentRepository AppointmentRepository => _appointmentRepo.Value;
        public IRepository<Patient> PatientRepository => _patientRepo.Value;
        public IRepository<Menu> MenuRepository => _menuRepo.Value;
        public IRoleMenuRepository RoleMenuRepository => _roleMenuRepo.Value;
        public IRolePermissionRepository RolePermissionRepository => _rolePermissionRepo.Value;
        public IQueueRepository QueueRepository => _queueRepo.Value;
        public IDoctorQueueRepository DoctorQueueRepository => _doctorQueueRepo.Value;
        // public IRepository<QueueEntry> QueueEntryRepository    => _queueEntryRepo.Value;
        
        // --------------------------------------------------------------------

        #region Commit / Save
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _dbContext.SaveChangesAsync(ct);   // Domain events dispatched inside DbContext
        #endregion

        #region Transactions  (matches IUnitOfWork signatures)
        public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_transaction is not null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
            }

            _transaction = await _dbContext.Database.BeginTransactionAsync(isolationLevel);
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                if (_transaction is not null)
                    await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction is not null)
                    await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction is not null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Convenience: save + commit in a single call.
        /// </summary>
        public async Task CompleteTransactionAsync(CancellationToken ct = default)
        {
            await SaveChangesAsync(ct);
            await CommitTransactionAsync();
        }
        #endregion

        #region Disposal
        public async ValueTask DisposeAsync()
        {
            if (_transaction is not null)
                await _transaction.DisposeAsync();

            await _dbContext.DisposeAsync();
        }

        void IDisposable.Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
        #endregion
    }
}