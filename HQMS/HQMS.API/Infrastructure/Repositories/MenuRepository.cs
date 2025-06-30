
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Interfaces;
using HQMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HQMS.API.Infrastructure.Repositories
{
    public class MenuRepository : IRepository<Menu>
    {
        private readonly ApplicationDbContext _context;

        public MenuRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Menu entity)
        {
            await _context.Menus.AddAsync(entity);
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return 0;

            _context.Menus.Remove(menu);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Menu>> GetAllAsync()
        {
            return await _context.Menus
                .Include(m => m.RoleMenus)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Menu> GetByIdAsync(Guid id)
        {
            return await _context.Menus
                .Include(m => m.RoleMenus)
                .FirstOrDefaultAsync(m => m.MenuId == id);
        }

        public async Task<int> UpdateAsync(Menu entity)
        {
            _context.Menus.Update(entity);
            return await _context.SaveChangesAsync();
        }

        //public async Task<bool> AssignMenusToRoleAsync(Guid roleId, List<Guid> menuIds)
        //{
        //    var existing = await _context.RoleMenus
        //        .Where(r => r.RoleId == roleId.ToString()) // Convert Guid to string for comparison
        //        .ToListAsync();

        //    _context.RoleMenus.RemoveRange(existing);

        //    var newAssignments = menuIds.Select(menuId => new RoleMenu(roleId.ToString(), menuId)).ToList(); // Ensure RoleId is passed as string

        //    await _context.RoleMenus.AddRangeAsync(newAssignments);
        //    await _context.SaveChangesAsync();

        //    return true;
        //}
        public async Task<bool> AssignMenusToRoleAsync(string roleId, List<Guid> menuIds)
        {
            var roleIdStr = roleId.ToString();

            var existing = await _context.RoleMenus
                .Where(r => r.RoleId == roleIdStr)
                .ToListAsync();

            _context.RoleMenus.RemoveRange(existing);

            var newAssignments = menuIds.Select(menuId => new RoleMenu(roleIdStr, menuId)).ToList();

            await _context.RoleMenus.AddRangeAsync(newAssignments);
            await _context.SaveChangesAsync();

            return true;
        }

        public Task RemoveRange(IEnumerable<Menu> entities)
        {
            _context.Menus.RemoveRange(entities);
            return Task.CompletedTask;
        }
    }
}
