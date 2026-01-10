using HireHuntBackend.Common;
using HireHuntBackend.Common.Dto;
using HireHuntBackend.Context;
using HireHuntBackend.Model;
using HireHuntBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text.Json;
using static HireHuntBackend.Common.Enums;

namespace HireHuntBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecruiterController : ControllerBase

    {
        private readonly JwtTokenService _jwtTokenService;
        private readonly DbContextHireHunt _dbContextHireHunt;
        public RecruiterController(JwtTokenService jwtTokenService, DbContextHireHunt dbContextHireHunt) { 
            _jwtTokenService = jwtTokenService;
            _dbContextHireHunt = dbContextHireHunt;
        }
        [Authorize]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RecuiterRegisterDto dto)
        {
            if(dto == null)
            {
                return BadRequest();
            }
            if(string.IsNullOrWhiteSpace(dto.CompanyName)) { 
               return BadRequest(new {
                   error="Company Name is Required"
               });
            }
            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                return BadRequest(new
                {
                    error = "FirstName is Required"
                });
            }
            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                return BadRequest(new
                {
                    error = "LastName is Required"
                });
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            //Save Recruiter details
            var recruiter = new Recruiter
            {
                CompanyName = dto.CompanyName,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                WebsiteUrl = dto.WebsiteUrl

            };
            _dbContextHireHunt.Recruiters.Add(recruiter);
            await _dbContextHireHunt.SaveChangesAsync();
            var result = await _jwtTokenService.GenerateToken(userId,email, Roles.Recruiter.ToString());
            Helper.SetAuthCookies(Response, result.RefreshToken, dto);
            return Ok(new {
                result.AccessToken 
            });

        }
        [Authorize(Roles="Recruiter")]
        [HttpPost("CreateJob")]
        public async Task<IActionResult> CreateJob( JobPostDto  dto)
        {
            if (dto == null) return BadRequest();

            if (string.IsNullOrWhiteSpace(dto.Title)) return BadRequest("Job Title is Required");
            if (string.IsNullOrWhiteSpace(dto.Company)) return BadRequest("Company is Required");
            if (string.IsNullOrWhiteSpace(dto.Location)) return BadRequest("Job Location is Required");
            if (string.IsNullOrWhiteSpace(dto.Description)) return BadRequest("Job Description is Required");


            return Ok(new { });
        }
    }
}
