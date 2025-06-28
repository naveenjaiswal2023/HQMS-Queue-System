
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Interfaces;
using HQMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HQMS.API.Infrastructure.Repositories
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly ApplicationDbContext _context;

        public RolePermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> GetPermissionIdsByRoleAsync(string roleId)
        {
            return await _context.RolePermissions
                .Where(r => r.RoleId == roleId)
                .Select(r => r.PermissionId)
                .ToListAsync();
        }

        public async Task<List<RolePermission>> GetRolePermissionsWithMenuAndPermissionAsync(Guid roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId.ToString()) // Convert Guid to string for comparison
                .Include(rp => rp.Permission)
                    .ThenInclude(p => p.Menu)
                .ToListAsync();
        }

        public async Task AssignPermissionsToRoleAsync(string roleId, List<int> permissionIds)
        {
            var existingPermissions = await _context.RolePermissions
                .Where(r => r.RoleId == roleId)
                .ToListAsync();

            _context.RolePermissions.RemoveRange(existingPermissions);

            var newAssignments = permissionIds
                .Select(pid => new RolePermission(roleId, pid)) // ✅ Use constructor, not initializer
                .ToList();

            await _context.RolePermissions.AddRangeAsync(newAssignments);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RolePermission>> GetAllAsync()
        {
            return await _context.RolePermissions.ToListAsync();
        }
        public async Task<RolePermission> GetByIdAsync(Guid roleId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Role)
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId.ToString()); // Convert Guid to string for comparison
        }

        public async Task AddAsync(RolePermission entity)
        {
            await _context.RolePermissions.AddAsync(entity);
        }
        public async Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId.ToString()) // Convert Guid to string for comparison
                .ToListAsync();
        }


        public async Task<int> UpdateAsync(RolePermission entity)
        {
            _context.RolePermissions.Update(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.RolePermissions.Remove(entity);
                return await _context.SaveChangesAsync();
            }

            return 0;
        }

        public Task RemoveRange(IEnumerable<RolePermission> entities)
        {
            _context.RolePermissions.RemoveRange(entities);
            return Task.CompletedTask;
        }
    }
}
