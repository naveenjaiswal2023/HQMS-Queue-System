using HQMS.API.Application.CommandModel;
using HQMS.API.Application.DTO;
using HQMS.API.Application.QueriesMode;
using HQMS.API.WebAPI.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HQMS.WebAPI.Controllers
{
    
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
            return Ok(result);
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> GetDashboard([FromQuery] QueueDashboardRequest request)
        {
            var query = new GetQueueDashboardQuery
            {
                HospitalId = request.HospitalId,
                DepartmentId = request.DepartmentId,
                DoctorIds = request.DoctorIds
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

    }
}
