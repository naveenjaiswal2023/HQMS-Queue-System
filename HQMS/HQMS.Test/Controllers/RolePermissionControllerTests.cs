using HQMS.Test.Models;
using HQMS.Tests.Common;
using HQMS.UI.Controllers;
using HQMS.UI.Interfaces;
using HQMS.UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace HQMS.Tests.Controllers
{
    public class RolePermissionControllerTests : ControllerTestBase
    {
        private readonly Mock<IRolePermissionService> _mockPermissionService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IRoleService> _mockRoleService;
        private readonly RolePermissionController _controller;
        private readonly Mock<ISession> _mockSession;

        public RolePermissionControllerTests()
        {
            _mockPermissionService = new Mock<IRolePermissionService>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockRoleService = new Mock<IRoleService>();
            _mockSession = new Mock<ISession>();

            var httpContext = new DefaultHttpContext();
            httpContext.Session = _mockSession.Object;
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            _controller = new RolePermissionController(
                _mockPermissionService.Object,
                _mockHttpContextAccessor.Object,
                _mockRoleService.Object
                
            );
            SetUserContext(_controller);
        }

    //    [Fact]
    //    public async Task AssignPermissions_Get_WithRoleId_LoadsPermissions()
    //    {
    //        // Arrange
    //        var roleId = Guid.NewGuid();
    //        var permissionId = Guid.NewGuid();
    //        var menuId = Guid.NewGuid();

    //        _mockRoleService.Setup(r => r.GetRolesAsync()).ReturnsAsync(new List<SelectListItem>
    //{
    //    new SelectListItem { Value = roleId.ToString(), Text = "Admin" }
    //});

    //        _mockPermissionService.Setup(s => s.GetPermissionsByRoleAsync(roleId)).ReturnsAsync(
    //            new RolePermissionViewModel
    //            {
    //                roleName = "Admin",
    //                Permissions = new List<PermissionWithMenu>
    //                {
    //            new PermissionWithMenu
    //            {
    //                permissionId = permissionId,
    //                permissionName = "View",
    //                Menu = new Menu
    //                {
    //                    menuId = menuId.ToString(), // Controller parses it as Guid
    //                    Name = "Dashboard"
    //                }
    //            }
    //                }
    //            });

    //        // Act
    //        var result = await _controller.AssignPermissions(roleId);

    //        // Assert
    //        var viewResult = Assert.IsType<ViewResult>(result);
    //        var model = Assert.IsType<RolePermissionAssignmentViewModel>(viewResult.Model);
    //        Assert.Equal("Admin", model.RoleName);
    //        Assert.Single(model.MenuPermissions);

    //        var menu = model.MenuPermissions[0];
    //        Assert.Equal("Dashboard", menu.MenuName);
    //        Assert.Single(menu.Permissions);
    //        Assert.Equal("View", menu.Permissions[0].PermissionName);
    //        Assert.True(menu.Permissions[0].IsAssigned);
    //    }

        [Fact]
        public async Task AssignPermissions_Post_AssignsAndRedirects()
        {
            var roleId = Guid.NewGuid().ToString();
            var permissionId = 123; // Changed to an integer to match the type of PermissionId

            var model = new RolePermissionAssignmentViewModel
            {
                RoleId = roleId,
                MenuPermissions = new List<MenuPermissionOption>
                {
                    new MenuPermissionOption
                    {
                        MenuId = Guid.NewGuid(),
                        MenuName = "Dashboard",
                        Permissions = new List<PermissionCheckbox>
                        {
                            new PermissionCheckbox
                            {
                                PermissionId = permissionId, // Fixed type mismatch
                                IsAssigned = true
                            }
                        }
                    }
                }
            };

            var result = await _controller.AssignPermissions(model);

            _mockPermissionService.Verify(s => s.AssignPermissionsToRole(roleId, It.Is<List<int>>(l => l.Contains(permissionId))), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AssignPermissions", redirect.ActionName);
            Assert.Equal(roleId, redirect.RouteValues["roleId"]);
        }
    }
}
