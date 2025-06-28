namespace HQMS.API.Application.DTO
{
    public class PermissionWithMenu
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public MenuDto Menu { get; set; }
    }
}
