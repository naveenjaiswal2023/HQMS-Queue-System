using HQMS.API.Domain.Events;

namespace HQMS.API.Domain.Entities
{
    public class Menu : BaseEntity
    {
        public Guid MenuId { get; private set; } // PascalCase for public property
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string Icon { get; private set; }
        public int OrderBy { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsVisible { get; private set; }
        public string PermissionKey { get; private set; }
        public Guid? ParentId { get; private set; }

        public List<RoleMenu> RoleMenus { get; private set; } = new();

        public Menu(string name, string url, string icon, int orderBy, string permissionKey, Guid? parentId = null)
        {
            MenuId = Guid.NewGuid();
            Name = name;
            Url = url;
            Icon = icon;
            OrderBy = orderBy;
            IsActive = true;
            IsVisible = true;
            PermissionKey = permissionKey;
            ParentId = parentId;

            AddDomainEvent(new MenuCreatedEvent(
            MenuId,
            Name,
            Url,
            Icon,
            OrderBy,
            IsActive,
            IsVisible,
            PermissionKey,
            ParentId));

        }

        // ✅ AddRole method for assigning roles
        public void AddRole(string roleId)
        {
            if (!RoleMenus.Any(rm => rm.RoleId == roleId))
            {
                RoleMenus.Add(new RoleMenu(roleId, MenuId));
            }
        }
    }
}
