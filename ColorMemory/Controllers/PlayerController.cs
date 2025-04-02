using ColorMemory.Data;
using ColorMemory.DTO;
using ColorMemory.Repository.Implementations;
using ColorMemory.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ColorMemory.Controllers
{
    [Route("api/player/")]
    [ApiController]
    public partial class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> _logger;
        private readonly PlayerService _playerService;
        private readonly ScoreService _scoreService;
        private readonly ArtworkService _artworkService;
        private readonly MoneyService _moneyService;
        private readonly HintService _hintService;
        public PlayerController(ILogger<PlayerController> logger, PlayerService playerService, ScoreService scoreService, ArtworkService artworkService, MoneyService moneyService, HintService hintService)
        {
            _logger = logger;
            _playerService = playerService;
            _scoreService = scoreService;
            _artworkService = artworkService;
            _moneyService = moneyService;
            _hintService = hintService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddPlayerAsync([FromBody] PlayerDTO playerInfo)
        {
            try
            {
                var player = await _playerService.AddPlayerAsync(playerInfo);

                if (player == null)
                {
                    _logger.LogWarning($"Player {playerInfo.PlayerId} already exists or could not be added.");
                    return Ok(false);
                }

                _logger.LogInformation($"Added player {playerInfo.PlayerId} to DB");

                await UpdateWeeklyScoreAsync(new ScoreDTO(playerInfo.PlayerId, playerInfo.Score));

                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding player {playerInfo.PlayerId}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{playerId}/money")]
        public async Task<int> GetPlayerMoneyAsync(string playerId)
        {
            var money = await _moneyService.GetPlayerMoneyAsync(playerId);
            return money;
        }

        [HttpPost("{playerId}/money/pay/{moneyToPay}")]
        public async Task<IActionResult> PayPlayerMoneyAsync(string playerId, int moneyToPay)
        {
            try
            {
                var response = await _moneyService.PayPlayerMoneyAsync(playerId, moneyToPay);
                _logger.LogInformation($"{playerId} paid {moneyToPay}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating money");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("{playerId}/money/earn/{moneyToEarn}")]
        public async Task<IActionResult> EarnPlayerMoneyAsync(string playerId, int moneyToEarn)
        {
            try
            {
                var response = await _moneyService.EarnPlayerMoneyAsync(playerId, moneyToEarn);
                _logger.LogInformation($"{playerId} earned {moneyToEarn}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating money");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("hint")]
        public async Task<IActionResult> BuyPlayerHintAsync([FromBody] HintDTO hintInfo)
        {
            try
            {
                var result = await _hintService.BuyPlayerHintAsync(hintInfo);
                _logger.LogInformation($"updated {hintInfo.PlayerId}'s hint");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating hint");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{playerId}/icon")]
        public async Task<IActionResult> GetPlayerIconIdAsync(string playerId)
        {
            var iconId = await _playerService.GetIconIdAsync(playerId);
            return Ok(iconId);
        }

        [HttpPost("{playerId}/icon/{iconId}")]
        public async Task<IActionResult> SetPlayerIconIdAsync(string playerId, int iconId)
        {
            var result = await _playerService.SetIconIdAsync(playerId, iconId);
            if (result)
            {
                _logger.LogInformation($"Updated {playerId}'s icon to {iconId}");
                return Ok(iconId);  // Return the updated icon ID if successful
            }
            else
            {
                _logger.LogWarning($"Failed to update {playerId}'s icon.");
                return StatusCode(500, new { error = "Failed to update the icon." });
            }
        }
    }
}
