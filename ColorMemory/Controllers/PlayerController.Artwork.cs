using ColorMemory.DTO;
using ColorMemory.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

namespace ColorMemory.Controllers
{
    public partial class PlayerController
    {
        [HttpPost("artwork/update")]
        public async Task<IActionResult> UpdatePlayerArtworkAsync([FromBody] PlayerArtworkDTO playerArtworkInfo)
        {
            var result = await _artworkService.UpdatePlayerArtworkAsync(playerArtworkInfo);

            if (result == null)
                return BadRequest(new
                {
                    Message = "Artwork could not be added.",
                    Reason = "The artwork might already be owned by the player or does not exist."
                });

            _logger.LogInformation($"updated {playerArtworkInfo.Title} to {playerArtworkInfo.PlayerId}");
            return Ok(new { rank = result.ToString() });
        }

        [HttpGet("{playerId}/artworks/owned")]
        public async Task<IActionResult> GetPlayerOwnedArtworksAsync(string playerId)
        {
            var artworks = await _artworkService.GetOwnedArtworksAsync(playerId);

            return Ok(artworks);
        }

        [HttpGet("{playerId}/artworks/{hasIt}")]
        public async Task<IActionResult> GetPlayerArtworksAsync(string playerId,    bool hasIt)
        {
            List<PlayerArtworkDTO> artworks;

            if (hasIt)
            {
                artworks = await _artworkService.GetOwnedArtworksAsync(playerId);
            }
            else
            {
                artworks = await _artworkService.GetUnownedArtworksAsync(playerId);

            }

            return Ok(artworks);
        }

        [HttpGet("{playerId}/artworks/whole")]
        public async Task<IActionResult> GetWholePlayerArtworksAsync(string playerId)
        {
            List<PlayerArtworkDTO> artworks = await _artworkService.GetWholePlayerArtworksAsync(playerId);

            return Ok(artworks);
        }
    }
}