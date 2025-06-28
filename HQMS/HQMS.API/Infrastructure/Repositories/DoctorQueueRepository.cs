
using HQMS.API.Domain.Interfaces;
using HQMS.Domain.Entities;
using HQMS.Domain.Interfaces;
using HQMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HQMS.Infrastructure.Repositories
{
    public class DoctorQueueRepository : IDoctorQueueRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorQueueRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(DoctorQueue doctorQueue)
        {
            await _context.DoctorQueues.AddAsync(doctorQueue);
        }

        public async Task<DoctorQueue?> GetByIdAsync(Guid id)
        {
            return await _context.DoctorQueues.FindAsync(id);
        }

        public async Task<IEnumerable<DoctorQueue>> GetAllAsync()
        {
            return await _context.DoctorQueues.ToListAsync();
        }

        public async Task<int> UpdateAsync(DoctorQueue doctorQueue)
        {
            _context.DoctorQueues.Update(doctorQueue);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            var entity = await _context.DoctorQueues.FindAsync(id);
            if (entity != null)
            {
                _context.DoctorQueues.Remove(entity);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<DoctorQueue?> GetByDoctorIdAsync(Guid doctorId)
        {
            return await _context.DoctorQueues.FirstOrDefaultAsync(q => q.DoctorId == doctorId);
        }

        public Task RemoveRange(IEnumerable<DoctorQueue> entities)
        {
            throw new NotImplementedException();
        }
    }
}
