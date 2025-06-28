
using HQMS.UI.Interfaces;
using HQMS.UI.Models;
using HQMS.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HQMS.UI.Services
{
    public class MenuService : IMenuService
    {
        private readonly ApiService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MenuService> _logger;

        public MenuService(ApiService apiService, IHttpContextAccessor httpContextAccessor,
                           IConfiguration configuration, ILogger<MenuService> logger)
        {
            _apiService = apiService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<List<MenuModel>> GetMenuHierarchyAsync()
        {
            try
            {
                // 🔐 Get roleId from claims (assuming role ID is stored)
                var roleId = _httpContextAccessor.HttpContext?.Session?.GetString("RoleId");

                if (string.IsNullOrEmpty(roleId))
                {
                    _logger.LogWarning("Role ID not found in claims.");
                    return new List<MenuModel>();
                }

                // 🔗 Call API using correct endpoint
                var response = await _apiService.GetAsync<List<MenuModel>>($"Menu/GetMenusByRoleId/{roleId}");


                if (response?.Data == null || !response.Data.Any())
                    return new List<MenuModel>();

                return BuildHierarchy(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load menus.");
                return new List<MenuModel>();
            }
        }

        private List<MenuModel> BuildHierarchy(List<MenuModel> flatMenus)
        {
            var lookup = flatMenus.ToDictionary(x => x.MenuId);

            foreach (var menu in flatMenus)
            {
                menu.Children = new List<MenuModel>(); // Ensure initialized
            }

            foreach (var menu in flatMenus)
            {
                if (menu.ParentId != null && lookup.TryGetValue(menu.ParentId.Value, out var parent))
                {
                    parent.Children.Add(menu);
                }
            }

            // Recursive sort
            void SortMenus(List<MenuModel> menus)
            {
                menus.Sort((a, b) => a.OrderBy.CompareTo(b.OrderBy));
                foreach (var menu in menus)
                {
                    SortMenus(menu.Children);
                }
            }

            // Get only root menus (ParentId == null) and sort
            var rootMenus = flatMenus
                .Where(m => m.ParentId == null)
                .OrderBy(m => m.OrderBy)
                .ToList();

            // Sort children of root menus
            SortMenus(rootMenus);

            return rootMenus;
        }
    }
}
