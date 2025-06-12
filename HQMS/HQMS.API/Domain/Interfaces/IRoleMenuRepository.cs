using HQMS.API.Domain.Entities;
using System.Threading.Tasks;

namespace HQMS.API.Domain.Interfaces
{
    public interface IRoleMenuRepository : IRepository<RoleMenu>
    {
        Task<IEnumerable<RoleMenu>> GetByRoleIdAsync(Guid roleId);
    }
}
