using ColorMemory.DTO;
using ColorMemory.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ColorMemory.Controllers
{
    public partial class PlayerController
    {
        [HttpPost("score/weekly/update")]
        public async Task<IActionResult> UpdateWeeklyScoreAsync([FromBody] ScoreDTO scoreInfo)
        {
            try
            {
                var result = await _scoreService.UpdateWeeklyScoreAsync(scoreInfo);
                _logger.LogInformation($"updated {scoreInfo.PlayerId}'s weekly score");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating score");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("score/national/update")]
        public async Task<IActionResult> UpdateNationalScoreAsync([FromBody] ScoreDTO scoreInfo)
        {
            try
            {
                await _scoreService.UpdateNationalScoreAsync(scoreInfo);
                _logger.LogInformation($"updated {scoreInfo.PlayerId}'s national score");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating score");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{playerId}/score/weekly")]
        public async Task<IActionResult> GetPlayerWeeklyScoreAsync(string playerId, [FromQuery] string type)
        {
            if (type == "int")
            {
                var scores = await _scoreService.GetWeeklyScoreAsIntAsync(playerId);
                return Ok(scores);
            }
            else
            {
                var playerScore = await _scoreService.GetWeeklyScoreAsDTOAsync(playerId);
                return Ok(playerScore);
            }
        }

        [HttpGet("{playerId}/score/national")]
        public async Task<IActionResult> GetPlayerNationalScoreAsync(string playerId)
        {
            var scores = await _scoreService.GetNationalScoreAsync(playerId);
            return Ok(scores);
        }

        [HttpGet("{playerId}/score/weekly/surrounding/{range}")]
        public async Task<IActionResult> GetSurroundingWeeklyRankingAsync(string playerId, int range)
        {
            var scores = await _scoreService.GetSurroundingWeeklyScoresAsync(playerId, range);
            return Ok(scores);
        }

        [HttpGet("{playerId}/ranking/weekly")]
        public async Task<IActionResult> GetPlayerWeeklyRankingAsync(string playerId)
        {
            int rank = await _scoreService.GetPlayerWeeklyRankingAsync(playerId);
            return Ok(rank);
        }
    }
}