namespace HQMS.API.Application.DTO
{
    public class MenuPermissionGroup
    {
        public Guid MenuId { get; set; }
        public string MenuName { get; set; }
        public List<PermissionCheckbox> Permissions { get; set; } = new();
    }
}
