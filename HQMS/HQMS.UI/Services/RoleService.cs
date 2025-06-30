using HQMS.Web.Models;
using HQMS.Web.Services;
using HQMS.UI.Interfaces;
using HQMS.UI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace HQMS.UI.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApiService _apiService; // your wrapper for HttpClient

        public RoleService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<SelectListItem>> GetRolesAsync()
        {
            var response = await _apiService.GetAsync<List<RoleDto>>("Roles");

            if (response != null && response.IsSuccess && response.Data != null) // Ensure Data is not null
            {
                return response.Data.Select(r => new SelectListItem
                {
                    Value = r.RoleId,
                    Text = r.roleName
                }).ToList();
            }

            return new List<SelectListItem>();
        }
    }

}
