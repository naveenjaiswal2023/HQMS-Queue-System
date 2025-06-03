using HospitalQueueSystem.Domain.Entities;
using HospitalQueueSystem.Infrastructure.Data;
using HQMS.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalQueueSystem.Infrastructure.Repositories
{
    public class DoctorQueueRepository : IRepository<DoctorQueue>
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

        public async Task<DoctorQueue?> GetByIdAsync(string id)
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
            return await _context.SaveChangesAsync(); // returns affected rows
        }

        public async Task<int> DeleteAsync(string id)
        {
            var entity = await _context.DoctorQueues.FindAsync(id);
            if (entity != null)
            {
                _context.DoctorQueues.Remove(entity);
                return await _context.SaveChangesAsync();
            }
            return 0; // nothing deleted
        }

        // Optional: Custom method specific to DoctorQueue
        public async Task<DoctorQueue?> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.DoctorQueues
                .FirstOrDefaultAsync(q => q.DoctorId == doctorId); // assuming DoctorId is int
        }
    }
}
