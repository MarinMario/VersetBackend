using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VersuriAPI.Data;
using VersuriAPI.Models.Dtos;
using VersuriAPI.Models.Entities;

namespace VersuriAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public SongsController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]
        public async Task<IActionResult> getAllAsync([FromHeader] string idToken)
        {
            var authorized = await Utils.Authorization.Validate(idToken);
            if (!authorized) return Unauthorized("Authorization Token is Invalid.");

            //var songs = dbContext.Songs.Include(s => s.User).ToList();
            return Ok("this worked");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getAsync([FromHeader] string idToken, Guid id)
        {
            var authorized = await Utils.Authorization.Validate(idToken);
            if (!authorized) return Unauthorized("Authorization Token is Invalid.");

            var song = dbContext.Songs.Find(id);

            if (song == null)
            {
                return BadRequest("Song doesn't exist");
            }

            return Ok(song);
        }

        [HttpPost]
        public async Task<IActionResult> addAsync([FromHeader] string idToken, SongDto songDto)
        {
            var authorized = await Utils.Authorization.Validate(idToken);
            if (!authorized) return Unauthorized("Authorization Token is Invalid.");

            var userGmail = dbContext.Users.Find(songDto.UserGmail);
            if (userGmail == null)
            {
                return BadRequest("User doesn't exist.");
            }

            var song = new Song
            {
                Id = new Guid(),
                Name = songDto.Name,
                Lyrics = songDto.Lyrics,
                User = userGmail
            };

            dbContext.Songs.Add(song);
            dbContext.SaveChanges();

            return Ok(song);
        }
    }
}
