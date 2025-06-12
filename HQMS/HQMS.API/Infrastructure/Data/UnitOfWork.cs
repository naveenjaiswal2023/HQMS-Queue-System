using HospitalQueueSystem.Domain.Entities;
using HospitalQueueSystem.Domain.Interfaces;
using HospitalQueueSystem.Infrastructure.Repositories;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Interfaces;
using HQMS.API.Infrastructure.Repositories;
using HQMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace HospitalQueueSystem.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction _transaction;
        private IRepository<Menu> _menuRepository;
        private IRoleMenuRepository _roleMenuRepository;

        private IRepository<Patient> _patientRepository;
        
        private IQueueRepository _queueRepository;
        private IRepository<DoctorQueue> _doctorQueueRepository;
        public IRepository<Patient> Patients { get; }
        public IRepository<Menu> Menus { get; }
        public IRepository<RoleMenu> RoleMenus { get; }
        public IRepository<DoctorQueue> Queues { get; }
        public IRepository<QueueEntry> QueuesEntries { get; }
        

        public ApplicationDbContext Context => _dbContext;

        public IRepository<Patient> PatientRepository
        {
            get
            {
                if (_patientRepository == null)
                    _patientRepository = new PatientRepository(_dbContext);
                return _patientRepository;
            }
        }
        public IRepository<Menu> MenuRepository
        {
            get
            {
                if (_menuRepository == null)
                    _menuRepository = new MenuRepository(_dbContext);
                return _menuRepository;
            }
        }
        public IRoleMenuRepository RoleMenuRepository
        {
            get
            {
                if (_roleMenuRepository == null)
                    _roleMenuRepository = new RoleMenuRepository(_dbContext);
                return _roleMenuRepository;
            }
        }

        //public IRepository<Patient> PatientRepository => throw new NotImplementedException();

        //public IRepository<QueueEntry> QueueRepository => throw new NotImplementedException();

        IQueueRepository IUnitOfWork.QueueRepository => throw new NotImplementedException();

        // public IPatientRepository Patients => throw new NotImplementedException();


        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Patients = new PatientRepository(_dbContext);
            Queues = new QueueRepository(dbContext) as IRepository<DoctorQueue>;
            QueuesEntries = new QueueRepository(dbContext) as IRepository<QueueEntry>;
        }

        // Start a new transaction
        public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_transaction != null)
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
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch (Exception)
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving changes.", ex);
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _dbContext.Dispose();
        }
    }

}
