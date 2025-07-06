using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HQMS.Test.Models
{
    public class PermissionWithMenu
    {
        public Guid permissionId { get; set; }
        public string permissionName { get; set; }
        public Menu Menu { get; set; }
    }
}
