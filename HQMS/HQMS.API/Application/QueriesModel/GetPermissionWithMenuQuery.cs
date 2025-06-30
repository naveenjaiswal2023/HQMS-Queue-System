using HQMS.API.Application.DTO;

namespace HQMS.API.Application.QuerieModel
{
    public class GetPermissionWithMenuQuery
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public MenuDto Menu { get; set; }
    }
}
