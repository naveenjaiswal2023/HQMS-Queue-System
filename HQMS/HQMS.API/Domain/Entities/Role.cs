using HQMS.Domain.Entities.Common;

namespace HQMS.API.Domain.Entities
{
    public class Role : BaseEntity
    {
        public Guid RoleId { get; private set; }
        public string Name { get; private set; }

        public ICollection<RoleMenu> RoleMenus { get; private set; } = new List<RoleMenu>();

        private Role() { }

        public Role(string name)
        {
            RoleId = Guid.NewGuid();
            Name = name;
        }
    }
}
