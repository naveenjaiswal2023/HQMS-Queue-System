namespace HQMS.API.Application.DTO
{
    public class RolePermissionViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<PermissionWithMenu> Permissions { get; set; }
    }
}
