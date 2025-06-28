namespace HQMS.UI.Models
{
    public class MenuPermissionOption
    {
        public Guid MenuId { get; set; }
        public string MenuName { get; set; }
        public List<PermissionCheckbox> Permissions { get; set; }
    }
}
