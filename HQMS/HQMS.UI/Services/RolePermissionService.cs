using HQMS.Web.Models;
using HQMS.Web.Services;
using HQMS.UI.Interfaces;
using HQMS.UI.Models;

namespace HQMS.UI.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly ApiService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RolePermissionService(ApiService apiService, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _httpContextAccessor = httpContextAccessor;
        }

        //public Task AssignPermissionsToRole(string roleId, List<int> permissionIds)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<RolePermissionViewModel> GetPermissionsByRoleAsync(Guid roleId)
        {
            var response = await _apiService.GetAsync<RolePermissionViewModel>($"RolePermission/GetPermissionsByRole/{roleId}");
            return response?.Data;
        }


        public async Task AssignPermissionsToRole(string roleId, List<int> permissionIds)
        {
            var payload = new
            {
                RoleId = roleId,
                PermissionIds = permissionIds
            };

            var response = await _apiService.PostAsync<RolePermissionViewModel>($"RolePermission/AssignPermissionsToRole", payload);
            //return response?.Data;
        }
    }
}
