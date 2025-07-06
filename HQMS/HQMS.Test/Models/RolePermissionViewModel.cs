using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HQMS.Test.Models
{
    public class RolePermissionViewModel
    {
        public string roleName { get; set; }
        public List<PermissionWithMenu> Permissions { get; set; }
    }
}
