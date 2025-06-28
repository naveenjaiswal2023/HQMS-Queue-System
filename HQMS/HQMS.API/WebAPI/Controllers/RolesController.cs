
using HQMS.API.Application.CommandModel;
using HQMS.API.Application.QuerieModel;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HQMS.API.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllRolesQuery());
            if (result == null)
            {
                return NotFound(new ApiResponse<RoleCreatedEvent>
                {
                    IsSuccess = false,
                    ErrorMessage = "Role not found."
                });
            }
            var response = new ApiResponse<List<RoleCreatedEvent>>
            {
                IsSuccess = true,
                Data = result
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(id));
            if (result == null)
            {
                return NotFound(new ApiResponse<RoleCreatedEvent>
                {
                    IsSuccess = false,
                    ErrorMessage = "Role not found."
                });
            }

            var response = new ApiResponse<List<RoleUpdatedEvent>>
            {
                IsSuccess = true,
                Data = result
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
        {
            var result = await _mediator.Send(command);
            if (result == null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    IsSuccess = false,
                    ErrorMessage = "Role creation failed."
                });
            }
            return Ok(new ApiResponse<string> { IsSuccess = true, ErrorMessage = "Role updated successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] string newName)
        {
            var result = await _mediator.Send(new UpdateRoleCommand(id, newName));
            if (!result)
                return NotFound(new ApiResponse<string> { IsSuccess = false, ErrorMessage = "Role not found or update failed." });

            return Ok(new ApiResponse<string> { IsSuccess = true, ErrorMessage = "Role updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _mediator.Send(new DeleteRoleCommand(id));
            if (!result)
            {
                return NotFound(new ApiResponse<string>
                {
                    IsSuccess = false,
                    ErrorMessage = "Role not found or deletion failed."
                });
            }

            return Ok(new ApiResponse<string>
            {
                IsSuccess = true,
                Data = "Role deleted successfully."
            });
        }
    }
}
