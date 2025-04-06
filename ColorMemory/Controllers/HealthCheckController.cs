using ColorMemory.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ColorMemory.Controllers
{
    /// <summary>
    /// For AWS Heartchecking
    /// </summary>
    [Route("health")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly ILogger<HealthCheckController> _logger;

        public HealthCheckController(ILogger<HealthCheckController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetHealth()
        {
            return StatusCode(200);
        }
    }
}
