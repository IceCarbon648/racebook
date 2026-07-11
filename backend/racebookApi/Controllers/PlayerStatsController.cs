using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.Interfaces;
using System.Security.Claims;

namespace racebookApi.Controllers
{
    [Authorize]
    [Route("api/amax-player-stats")]
    [ApiController]
    public class PlayerStatsController : ControllerBase
    {
        private readonly IPlayerStatsSnapshotService _playerStatsSnapshotService;
        private readonly ISessionService _sessionService;

        public PlayerStatsController(IPlayerStatsSnapshotService playerStatsSnapshotService, ISessionService sessionService)
        {
            _playerStatsSnapshotService = playerStatsSnapshotService;
            _sessionService = sessionService;
        }

        [HttpPost("open-session")]
        public async Task<IActionResult> OpenStatsSession([FromBody] string sessionName)
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
            string? amaxUsername = User.FindFirst(ClaimTypes.GivenName)?.Value.ToString();
            Guid snapshotId = await _playerStatsSnapshotService.SaveSnapshot(amaxUsername);

            await _sessionService.OpenSession(uid, sessionName, snapshotId);

            return Ok(new { message = "Session opened successfully" });
        }

        [HttpPost("close-session")]
        public async Task<IActionResult> CloseStatsSession([FromBody] string sessionId)
        {
            string? amaxUsername = User.FindFirst(ClaimTypes.GivenName)?.Value.ToString();
            Guid snapshotId = await _playerStatsSnapshotService.SaveSnapshot(amaxUsername);

            await _sessionService.CloseSession(sessionId, snapshotId);

            return Ok(new { message = "Session closed successfully" });
        }
    }
}