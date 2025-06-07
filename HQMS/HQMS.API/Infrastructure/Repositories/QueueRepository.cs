using HospitalQueueSystem.Domain.Entities;
using HospitalQueueSystem.Domain.Interfaces;
using HospitalQueueSystem.Infrastructure.Data;
using HQMS.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalQueueSystem.Infrastructure.Repositories
{
    public class QueueRepository : IRepository<QueueEntry>
    {
        private readonly ApplicationDbContext _context;

        public QueueRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<QueueEntry?> GetByIdAsync(string id)
        {
            return await _context.QueueEntries.FindAsync(id);
        }

        public async Task<List<QueueEntry>> GetQueueByDoctorIdAsync(int doctorId)
        {
            return await _context.QueueEntries
                .Where(q => q.DoctorId == doctorId.ToString() && q.Status == "Called")
                .OrderBy(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(QueueEntry entry)
        {
            await _context.QueueEntries.AddAsync(entry);
        }

        public async Task<int> UpdateAsync(QueueEntry entity)
        {
            _context.QueueEntries.Update(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<QueueEntry>> GetAllAsync()
        {
            return await _context.QueueEntries.ToListAsync();
        }

        public async Task<int> DeleteAsync(string id)
        {
            var entity = await _context.QueueEntries.FindAsync(id);
            if (entity != null)
            {
                _context.QueueEntries.Remove(entity);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }
    }
}
