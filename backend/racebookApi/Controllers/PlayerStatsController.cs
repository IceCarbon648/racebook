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

        public PlayerStatsController(IPlayerStatsSnapshotService playerStatsSnapshotService)
        {
            _playerStatsSnapshotService = playerStatsSnapshotService;
        }

        [HttpPost]
        public async Task<IActionResult> SaveStatsSnapshot()
        {


            return Ok();
        }
    }
}