using Microsoft.AspNetCore.Mvc;
using racebookApi.Repositories.Interfaces;
using racebookApi.Services.Interfaces;

namespace racebookApi.Controllers
{
    [Route("api/[controller]")]
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
            Guid snapshotId = await _playerStatsSnapshotService.SaveSnapshot("IceCarbon");

            await _sessionService.OpenSession("9D51DE57-A958-4B74-B975-52A5F81C7F93", sessionName, snapshotId);

            return Ok();
        }

        [HttpPost("close-session")]
        public async Task<IActionResult> CloseStatsSession([FromBody] string sessionId)
        {
            Guid snapshotId = await _playerStatsSnapshotService.SaveSnapshot("IceCarbon");

            await _sessionService.CloseSession(sessionId, snapshotId);

            return Ok();
        }
    }
}