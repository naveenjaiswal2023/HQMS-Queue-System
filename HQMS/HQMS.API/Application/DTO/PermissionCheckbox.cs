namespace HQMS.API.Application.DTO
{
    public class PermissionCheckbox
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public bool IsAssigned { get; set; } // for checked binding
    }
}
