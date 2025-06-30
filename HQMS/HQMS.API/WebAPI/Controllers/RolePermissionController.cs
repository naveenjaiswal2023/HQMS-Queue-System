using HQMS.API.Application.CommandModel;
using HQMS.API.Application.DTO;
using HQMS.API.Application.QuerieModel;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HQMS.API.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolePermissionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RolePermissionController> _logger;

        public RolePermissionController(IMediator mediator, ILogger<RolePermissionController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("AssignPermissionsToRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignPermissionsToRole([FromBody] AssignPermissionsCommand command)
        {
            if (command == null || string.IsNullOrEmpty(command.RoleId) || command.PermissionIds == null)
            {
                return BadRequest("Invalid input.");
            }

            try
            {
                var result = await _mediator.Send(command);

                if (result)
                {
                    return Ok(new
                    {
                        IsSuccess = true,
                        Message = "Permissions assigned successfully."
                    });
                }

                return StatusCode(500, new
                {
                    IsSuccess = false,
                    Message = "Failed to assign permissions."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permissions to role.");
                return StatusCode(500, new
                {
                    IsSuccess = false,
                    Message = "Internal server error."
                });
            }
        }
        [HttpGet("GetPermissionsByRole/{roleId}")]
        public async Task<IActionResult> GetPermissionsByRole(Guid roleId)
        {
            try
            {
                var query = new GetPermissionsByRoleQuery(roleId);
                var result = await _mediator.Send(query);

                if (result == null)
                    return NotFound($"No permissions found for RoleId: {roleId}");

                var response = new ApiResponse<RolePermissionViewModel> // ✅ Fix here
                {
                    IsSuccess = true,
                    Data = result
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching permissions for RoleId: {RoleId}", roleId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
