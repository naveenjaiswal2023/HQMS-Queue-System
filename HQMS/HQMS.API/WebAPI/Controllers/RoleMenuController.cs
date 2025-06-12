using HQMS.API.Application.CommandModel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HQMS.API.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleMenuController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoleMenuController(IMediator mediator) => _mediator = mediator;

        [HttpPost("assign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignMenus([FromBody] AssignMenusToRoleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
