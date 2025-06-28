using Microsoft.AspNetCore.Mvc.Rendering;

namespace HQMS.UI.Interfaces
{
    public interface IRoleService
    {
        Task<List<SelectListItem>> GetRolesAsync();
    }
}
