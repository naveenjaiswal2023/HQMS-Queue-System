namespace HQMS.API.Application.DTO
{
    public class MenuDto
    {
        public string MenuId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int OrderBy { get; set; }
        public string PermissionKey { get; set; }
        public Guid? ParentMenuId { get; set; }
    }
}
