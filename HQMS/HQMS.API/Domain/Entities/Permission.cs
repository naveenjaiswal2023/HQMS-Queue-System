using HospitalQueueSystem.Domain.Common;
using HQMS.Domain.Entities.Common;
using System;
using System.Collections.Generic;

namespace HQMS.API.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public Guid MenuId { get; private set; }
        public virtual Menu Menu { get; private set; }

        public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

        public Permission(string name, Guid menuId)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            MenuId = menuId;
        }

        private Permission() { } // For EF
    }
}