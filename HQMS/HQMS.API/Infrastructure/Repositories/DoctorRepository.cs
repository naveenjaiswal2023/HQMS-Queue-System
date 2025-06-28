using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Interfaces;
using HQMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HQMS.API.Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;
        public DoctorRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
        }
        public async Task<Doctor?> GetByIdAsync(Guid id)
        {
            return await _context.Doctors.FindAsync(id);
        }
        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctors.ToListAsync();
        }
        public async Task<int> UpdateAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            return await _context.SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(Guid id)
        {
            var entity = await _context.Doctors.FindAsync(id);
            if (entity != null)
            {
                _context.Doctors.Remove(entity);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public Task RemoveRange(IEnumerable<Doctor> entities)
        {
            throw new NotImplementedException();
        }
    }
}
