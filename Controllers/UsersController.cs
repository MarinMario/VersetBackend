using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VersuriAPI.Data;
using VersuriAPI.Models;
using VersuriAPI.Utils;

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

        //[HttpGet("GetAll")]
        //public IActionResult GetAll()
        //{
        //    var users = dbContext.Users.ToList();
        //    return Ok(users);
        //}

        [HttpGet("GetUserData")]
        public async Task<IActionResult> GetUserData([FromHeader] string idToken)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var user = Misc.getUserByEmail(dbContext, auth.Email);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("GetUserPublic/{userId}")]
        public async Task<IActionResult> GetUserData([FromHeader] string idToken, Guid userId)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var user = dbContext.Users.Find(userId);
            if (user == null)
                return NotFound();

            return Ok(Misc.UserToPublic(user));
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromHeader] string idToken)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var foundUser = Misc.getUserByEmail(dbContext, auth.Email);
            if (foundUser != null)
                return BadRequest("User already exist.");

            var newUser = new User
            {
                Id = new Guid(),
                CreationDate = DateTime.UtcNow,
                Email = auth.Email,
                Name = auth.Name,
                Public = false,
                LikedSongs = [],
                DislikedSongs = [],
            };

            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();

            return Ok(newUser);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromHeader] string idToken, DtoUserUpdate user)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var foundUser = Misc.getUserByEmail(dbContext, auth.Email);
            if (foundUser == null)
                return BadRequest("User doesn't exist.");

            foundUser.Name = user.Name;
            foundUser.Public = user.Public;

            dbContext.SaveChanges();

            return Ok(foundUser);
        }

        [HttpPost("ToggleRating/{ratingName}/{songId}")]
        public async Task<IActionResult> ToggleRating([FromHeader] string idToken, string ratingName, Guid songId)
        {

            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var allowedRating = new string[] { "Like", "Dislike" };
            if (!allowedRating.Contains(ratingName))
                return BadRequest("Endpoint must contain either Like or Dislike");

            var user = Misc.getUserByEmail(dbContext, auth.Email);
            if (user == null)
                return NotFound("User doesn't exist");

            if (ratingName == "Like")
            {
                Misc.ToggleFromList(user.LikedSongs, songId);
                user.DislikedSongs.Remove(songId);
            }
            else
            {
                Misc.ToggleFromList(user.DislikedSongs, songId);
                user.LikedSongs.Remove(songId);
            }

            dbContext.SaveChanges();
            return Ok(user);
        }
    }
}
