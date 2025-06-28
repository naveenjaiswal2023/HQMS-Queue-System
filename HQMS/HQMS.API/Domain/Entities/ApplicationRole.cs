using HQMS.Domain.Common;
using HQMS.API.Domain.Events;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace HQMS.API.Domain.Entities
{
    public class ApplicationRole : IdentityRole
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        // ✅ Add this for EF Core navigation
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();

        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(Guid.NewGuid().ToString())
        {
            Name = roleName;
            AddDomainEvent(new RoleCreatedEvent(Id, roleName));
        }

        public static ApplicationRole Create(string roleName)
        {
            return new ApplicationRole(roleName);
        }

        public void Update(string newName)
        {
            if (Name != newName)
            {
                Name = newName;
                AddDomainEvent(new RoleUpdatedEvent(Id, Name));
            }
        }

        public void MarkAsDeleted()
        {
            AddDomainEvent(new RoleDeletedEvent(Id, Name));
        }

        public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
