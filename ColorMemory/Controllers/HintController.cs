using ColorMemory.DTO;
using ColorMemory.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ColorMemory.Controllers
{
    [Route("api/hint")]
    [ApiController]
    public class HintController : ControllerBase
    {
        private readonly ILogger<ScoreController> _logger;
        private readonly HintService _hintService;

        public HintController(ILogger<ScoreController> logger, HintService hintService)
        {
            _logger = logger;
            _hintService = hintService;
        }

        [HttpGet("price")]
        public async Task<IActionResult> GetHintPriceAsync([FromQuery] HintType type)
        {
            int price = _hintService.GetHintPrice(type);
            return Ok(price);
        }
    }
}
