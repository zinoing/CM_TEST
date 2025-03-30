using ColorMemory.Repository.Implementations;
using ColorMemory.DTO;
using ColorMemory.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ColorMemory.Controllers
{
    [Route("api/artwork")]
    [ApiController]
    public class ArtworkController : ControllerBase
    {
        private readonly ILogger<ArtworkController> _logger;
        private readonly ArtworkService _artworkService;

        public ArtworkController(ILogger<ArtworkController> logger, ArtworkService artworkService)
        {
            _logger = logger;
            _artworkService = artworkService;
        }

        /// <summary>
        /// Only use by admin
        /// </summary>
        [HttpGet("{fileName}")]
        public async Task<IActionResult> AddArtworkAsync(string fileName)
        {
            var artwork = await _artworkService.AddArtworkAsync(fileName);

            if (artwork == null)
                _logger.LogInformation($"{fileName} already exists");
            else 
                _logger.LogInformation($"added {fileName} to db");

            return Ok(artwork);
        }
    }
}
