using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Interfaces;
using HQMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HQMS.API.Infrastructure.Repositories
{
    public class RoleMenuRepository : IRoleMenuRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleMenuRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RoleMenu entity)
        {
            await _context.RoleMenus.AddAsync(entity);
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            var roleMenu = await _context.RoleMenus.FindAsync(id);
            if (roleMenu == null) return 0;

            _context.RoleMenus.Remove(roleMenu);
            return await _context.SaveChangesAsync();  // Save inside UnitOfWork ideally
        }

        public async Task<IEnumerable<RoleMenu>> GetAllAsync()
        {
            return await _context.RoleMenus.ToListAsync();
        }

        public async Task<RoleMenu> GetByIdAsync(Guid id)
        {
            return await _context.RoleMenus.FindAsync(id);
        }

        public async Task<int> UpdateAsync(RoleMenu entity)
        {
            _context.RoleMenus.Update(entity);
            return await _context.SaveChangesAsync();  // Save inside UnitOfWork ideally
        }

        public async Task RemoveRange(IEnumerable<RoleMenu> entities)
        {
            _context.RoleMenus.RemoveRange(entities);
            // SaveChanges is deferred to UnitOfWork
        }

        public async Task<IEnumerable<RoleMenu>> GetByRoleIdAsync(Guid roleId)
        {
            return await _context.RoleMenus
                .Where(rm => rm.RoleId == roleId.ToString()) // if RoleId is string in DB
                .ToListAsync();
        }

    }
}
