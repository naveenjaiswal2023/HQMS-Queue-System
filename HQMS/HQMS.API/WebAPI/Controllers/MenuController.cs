using HQMS.API.Application.CommandModel;
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
    public class MenuController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MenuController> _logger;

        public MenuController(IMediator mediator, ILogger<MenuController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateMenu(CreateMenuCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign-to-role")]
        public async Task<IActionResult> AssignMenusToRole(AssignMenusToRoleCommand command)
        {
            var result = await _mediator.Send(command);

            var response = new ApiResponse<bool>
            {
                IsSuccess = true,
                Data = result
            };

            return Ok(response);
        }
        /// <summary>
        /// Get all menus assigned to a specific role.
        /// </summary>
        [HttpGet("GetMenusByRoleId/{roleId}")]
        public async Task<IActionResult> GetMenusByRoleId(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
            {
                _logger.LogWarning("RoleId parameter is missing.");
                return BadRequest("RoleId is required.");
            }

            var query = new GetMenusByRoleIdQuery(roleId); // ✅ correct constructor usage
            var result = await _mediator.Send(query);

            if (result == null || !result.Any())
            {
                return NotFound($"No menus found for RoleId: {roleId}");
            }
            var response = new ApiResponse<List<MenuCreatedEvent>>
            {
                IsSuccess = true,
                Data = result
            };

            return Ok(response);
        }
    }
}
