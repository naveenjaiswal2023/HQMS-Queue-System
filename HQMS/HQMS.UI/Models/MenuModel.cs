using System.Text.Json.Serialization;

namespace HQMS.UI.Models
{
    public class MenuModel
    {
        [JsonPropertyName("menuId")]
        public Guid MenuId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("orderBy")]
        public int OrderBy { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("isVisible")]
        public bool IsVisible { get; set; }

        [JsonPropertyName("permissionKey")]
        public string PermissionKey { get; set; }

        [JsonPropertyName("parentId")]
        public Guid? ParentId { get; set; }

        [JsonPropertyName("children")]

        public List<MenuModel> Children { get; set; } = new();
    }
}
