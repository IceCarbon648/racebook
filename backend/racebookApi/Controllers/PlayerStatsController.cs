using Microsoft.AspNetCore.Mvc;
using racebookApi.Repositories.Interfaces;

namespace racebookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerStatsController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> SaveStatsSnapshot()
        {


            return Ok();
        }
    }
}