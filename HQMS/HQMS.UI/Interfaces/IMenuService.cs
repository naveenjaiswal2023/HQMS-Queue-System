using HQMS.UI.Models;

namespace HQMS.UI.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuModel>> GetMenuHierarchyAsync();
    }
}
