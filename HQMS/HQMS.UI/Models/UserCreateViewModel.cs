using Microsoft.AspNetCore.Mvc.Rendering;

namespace HQMS.UI.Models
{
    public class UserCreateViewModel
    {
        public string SelectedRoleId { get; set; }

        public List<SelectListItem> Roles { get; set; }
    }

}
