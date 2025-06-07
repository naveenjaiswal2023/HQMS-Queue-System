using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HQMS.API.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ConfigController(IConfiguration configuration) => _configuration = configuration;

        [HttpGet]
        public IActionResult Get()
        {
            var hubUrl = _configuration["SignalR:HubUrl"];
            return Ok(new { hubUrl });
        }
    }
}
