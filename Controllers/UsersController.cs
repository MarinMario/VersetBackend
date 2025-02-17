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
        public IActionResult GetAll()
        {
            var users = dbContext.Users.ToList();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromHeader] string idToken)
        {
            var auth = await Utils.Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var foundUser = dbContext.Users.Find(auth.Email);
            if (foundUser != null)
                return BadRequest("User already exists.");

            var newUser = new User { CreationDate = DateTime.UtcNow, Email = auth.Email, Name = auth.Name, Public = false };

            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();

            return Ok(newUser);
        }
    }
}
