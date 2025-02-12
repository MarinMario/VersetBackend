using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VersuriAPI.Data;
using VersuriAPI.Models.Entities;

namespace VersuriAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly AppDbContext dbContext;

        public UsersController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> getAllAsync([FromHeader] string idToken)
        {

            var authorized = await Utils.Authorization.Validate(idToken);
            if (!authorized) return Unauthorized("Authorization Token is Invalid.");

            var users = dbContext.Users.ToList();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> addAsync([FromHeader] string idToken, [FromBody] User user)
        {
            var authorized = await Utils.Authorization.Validate(idToken);
            if (!authorized) return Unauthorized("Authorization Token is Invalid.");

            var foundUser = dbContext.Users.Find(user.Gmail);
            if (foundUser == null) {
                return BadRequest("User already exists.");
            }

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            return Ok(user);
        }
    }
}
