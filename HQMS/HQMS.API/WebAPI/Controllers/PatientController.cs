using Azure.Messaging.ServiceBus;
using HospitalQueueSystem.Application.CommandModel;
using HospitalQueueSystem.Application.Commands;
using HospitalQueueSystem.Application.DTO;
using HospitalQueueSystem.Application.Queries;
using HospitalQueueSystem.Application.Services;
using HospitalQueueSystem.Domain.Entities;
using HospitalQueueSystem.Domain.Events;
using HospitalQueueSystem.Domain.Interfaces;
using HQMS.API.Application.QuerieModel;
using HQMS.API.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HospitalQueueSystem.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IMediator _mediator;

        public PatientController(IMediator mediator, ILogger<PatientController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); // Ensure mediator is injected
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Ensure logger is injected
        }

        [HttpPost("RegisterPatient")]
        public async Task<IActionResult> RegisterPatient([FromBody] PatientRegisteredEvent model)
        {
            try
            {
                if (model == null ||
                    string.IsNullOrWhiteSpace(model.Name) ||
                    string.IsNullOrWhiteSpace(model.Gender) ||
                    string.IsNullOrWhiteSpace(model.Department) ||
                    model.Age <= 0)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid patient data."
                    });
                }

                var command = new RegisterPatientCommand(model.Name, model.Age, model.Gender, model.Department, model.PhoneNumber,model.Email,model.Address,model.BloodGroup,model.HospitalId,model.DoctorId);
                var result = await _mediator.Send(command);

                if (result)
                    return Ok(new ApiResponse<string>
                    {
                        IsSuccess = true,
                        ErrorMessage = "Patient registered successfully."
                    });
                else
                    return StatusCode(500, new ApiResponse<string>
                    {
                        IsSuccess = false,
                        ErrorMessage = "Registration failed due to an internal error."
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering patient");
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSuccess = false,
                    ErrorMessage = $"An unexpected error occurred: {ex.Message}"
                });
            }
        }


        [HttpGet("GetAllPatients")]
        public async Task<IActionResult> GetAllPatients()
        {
            try
            {
                var result = await _mediator.Send(new GetAllPatientsQuery());

                var response = new ApiResponse<List<PatientRegisteredEvent>>
                {
                    IsSuccess = true,
                    Data = result
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching patients.");

                return StatusCode(500, new ApiResponse<List<Patient>>
                {
                    IsSuccess = false,
                    ErrorMessage = "An error occurred while retrieving patients."
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetPatientByIdQuery(id));

                if (result == null || !result.Any())
                {
                    return NotFound($"Patient with ID '{id}' not found.");
                }
                var response = new ApiResponse<List<PatientUpdatedEvent>>
                {
                    IsSuccess = true,
                    Data = result
                };

                return Ok(response);
                //return Ok(result.First()); // Assuming you return a list with one patient
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching patient with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(string id, [FromBody] PatientUpdatedEvent model)
        {
            try
            {
                if (!Guid.TryParse(id, out var parsedId) || parsedId != model.PatientId)
                    return BadRequest(new ApiResponse<string> { IsSuccess = false, ErrorMessage = "Patient ID mismatch." });

                var command = new UpdatePatientCommand(model);
                var result = await _mediator.Send(command);

                if (!result)
                    return NotFound(new ApiResponse<string> { IsSuccess = false, ErrorMessage = "Patient not found or update failed." });

                return Ok(new ApiResponse<string> { IsSuccess = true, ErrorMessage = "Patient updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient with ID {PatientId}", id);
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, ErrorMessage = "An error occurred while updating the patient." });
            }
        }


        // DELETE: api/patient/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(string id)
        {
            try
            {
                var command = new DeletePatientCommand(id);
                var result = await _mediator.Send(command);

                if (!result)
                    return NotFound(new ApiResponse<string> {IsSuccess = false, ErrorMessage = "Patient not found or deletion failed." });

                return Ok(new ApiResponse<string> {IsSuccess = true, ErrorMessage = "Patient deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting patient with ID {PatientId}", id);
                return StatusCode(500, new ApiResponse<string> {IsSuccess = false, ErrorMessage = "An error occurred while deleting the patient." });
            }
        }

    }
}
