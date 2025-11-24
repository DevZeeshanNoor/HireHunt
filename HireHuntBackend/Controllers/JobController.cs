using HireHuntBackend.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HireHuntBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly DbContextHireHunt _context;
        public JobController(DbContextHireHunt context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetJobPosting()
        {
            var jobs = await _context.JobPosts.ToListAsync();
            return Ok(jobs);
        }
    }
}
