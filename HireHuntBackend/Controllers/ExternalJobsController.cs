using HireHuntBackend.Context;
using HireHuntBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HireHuntBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalJobsController : ControllerBase
    {
        private readonly DbContextHireHunt _context;
        private readonly ExternalJobsService _extJobsService;
        public ExternalJobsController(DbContextHireHunt context, ExternalJobsService externalJobsService)
        {
            _context = context;
            _extJobsService = externalJobsService;
        }

        [HttpGet("remoteok")]
        public async Task<IActionResult> GetRemoteOk()
        {
            var json=await _extJobsService.GetRemoteOk();
            return Ok(json);
        }

        [HttpGet("weWorkRemotely")]
        public async Task<IActionResult> WeWorkRemotely()
        {
            var json = await _extJobsService.WeWorkRemotely();
            return Ok(json);
        }
    }
}
