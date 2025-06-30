
using HQMS.Domain.Common;

namespace HQMS.API.Domain.Events
{
    public record MenuCreatedEvent : IDomainEvent
    {
        public Guid MenuId { get; private set; }
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string Icon { get; private set; }
        public int OrderBy { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsVisible { get; private set; }
        public string PermissionKey { get; private set; }
        public Guid? ParentId { get; private set; }

        public MenuCreatedEvent(
            Guid menuId,
            string name,
            string url,
            string icon,
            int orderBy,
            bool isActive,
            bool isVisible,
            string permissionKey,
            Guid? parentId)
        {
            MenuId = menuId;
            Name = name;
            Url = url;
            Icon = icon;
            OrderBy = orderBy;
            IsActive = isActive;
            IsVisible = isVisible;
            PermissionKey = permissionKey;
            ParentId = parentId;
        }
    }
}
