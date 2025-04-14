using ColorMemory.DTO;
using ColorMemory.Repository.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ColorMemory.Controllers;
using ColorMemory.Services;

namespace ColorMemory.Controllers
{
    [Route("api/score")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        private readonly ILogger<ScoreController> _logger;
        private readonly ScoreService _scoreService;

        public ScoreController(ILogger<ScoreController> logger, ScoreService scoreService)
        {
            _logger = logger;
            _scoreService = scoreService;
        }

        /// <summary>
        /// Get top scores from ranking data
        /// </summary>
        /// <returns></returns>
        [HttpGet("weekly/{count}")]
        public async Task<IActionResult> GetTopWeeklyScoresAsync(int count)
        {
            var scores = await _scoreService.GetTopWeeklyScoresAsync(count);
            return Ok(scores);
        }
    }
}
