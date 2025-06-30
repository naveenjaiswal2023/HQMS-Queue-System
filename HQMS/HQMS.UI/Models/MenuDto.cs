namespace HQMS.UI.Models
{
    public class MenuDto
    {
        public string menuId { get; set; }
        public string Name { get; set; }

        // Optional additional properties
        public string url { get; set; }
        public string icon { get; set; }
        public int orderBy { get; set; }
        public string permissionKey { get; set; }
        public string parentMenuId { get; set; }
    }
}
