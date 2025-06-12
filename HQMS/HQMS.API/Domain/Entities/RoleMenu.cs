using Microsoft.AspNetCore.Identity;

namespace HQMS.API.Domain.Entities
{
    public class RoleMenu : BaseEntity
    {
        public Guid RoleMenuID { get; private set; }
        public string RoleId { get; private set; }
        public Guid MenuId { get; private set; }

        // Navigation Properties
        public virtual Menu Menu { get; private set; }
        public virtual ApplicationRole Role { get; private set; }

        // Required by EF Core
        private RoleMenu() { }

        public RoleMenu(string roleId, Guid menuId)
        {
            RoleMenuID = Guid.NewGuid();
            RoleId = roleId;
            MenuId = menuId;
        }
    }
}