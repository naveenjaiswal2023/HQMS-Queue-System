using HQMS.API.Application.CommandModel;
using HQMS.API.Application.DTO;
using HQMS.API.Application.QueriesMode;
using HQMS.API.Domain.Entities;
using HQMS.API.WebAPI.Controllers;
using HQMS.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HQMS.WebAPI.Controllers
{
    [Authorize(Roles = "POD,Doctor")]
    public class QueueController : BaseApiController
    {
        //[HttpPost("process/{id}")]
        //public async Task<IActionResult> Process(Guid id)
        //{
        //    var result = await Mediator.Send(new ProcessQueueItemCommand(id));

        //    return result
        //        ? Ok(new { message = "Queue item processed." })
        //        : NotFound(new { message = "Item not found." });
        //}

        [HttpPost("GenerateQueue")]
        public async Task<IActionResult> GenerateQueue([FromBody] GenerateDoctorQueueCommand command)
        {
            if (command.DoctorId == Guid.Empty || command.PatientId == Guid.Empty)
            {
                return BadRequest("DoctorId and PatientId must be valid non-empty GUIDs.");
            }

            var result = await Mediator.Send(command);
            if (result)
                return Ok(new ApiResponse<string>
                {
                    IsSuccess = true,
                    ErrorMessage = "Queue generated successfully."
                });
            else
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSuccess = false,
                    ErrorMessage = "Registration failed due to an internal error."
                });
        }

        [HttpGet("GetQueueDashboard")]
        public async Task<IActionResult> GetDashboard([FromQuery] QueueDashboardRequest request)
        {
            var query = new GetQueueDashboardQuery
            {
                HospitalId = request.HospitalId,
                DepartmentId = request.DepartmentId,
                DoctorIds = request.DoctorIds
            };

            var result = await Mediator.Send(query);
            var response = new ApiResponse<List<QueueDashboardItemDto>>
            {
                IsSuccess = true,
                Data = result
            };
            return Ok(response);
            
        }

    }
}
