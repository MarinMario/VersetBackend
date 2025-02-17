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
        public IActionResult GetAll()
        {
            var songs = dbContext.Songs.Include(s => s.User).ToList();
            return Ok(songs);
        }


        [HttpGet("user")]
        public async Task<IActionResult> GetByUser([FromHeader] string idToken)
        {
            var auth = await Utils.Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var songs = dbContext.Songs.Include(s => s.User).ToList().FindAll(s => s.User.Email == auth.Email);

            if (songs == null)
                return BadRequest("idk why");

            return Ok(songs);
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById([FromHeader] string idToken, Guid id)
        {
            var auth = await Utils.Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var song = dbContext.Songs.Include(s => s.User).ToList().Find(s => s.Id == id);
            if (song == null)
                return BadRequest("Song doesn't exist");
            if (song.User.Email != auth.Email)
                return Unauthorized("You can't delete data from other users.");

            return Ok(song);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromHeader] string idToken, SongDto songDto)
        {
            var auth = await Utils.Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var user = dbContext.Users.Find(auth.Email);
            if (user == null)
                return BadRequest("User doesn't exist.");

            var utcNow = DateTime.UtcNow;

            var song = new Song
            {
                Id = new Guid(),
                Name = songDto.Name,
                Lyrics = songDto.Lyrics,
                Description = songDto.Description,
                User = user,
                CreationDate = utcNow,
                LastUpdateDate = utcNow,
                AccessFor = songDto.AccessFor
            };


            dbContext.Songs.Add(song);
            dbContext.SaveChanges();

            return Ok(song);
        }

        [HttpDelete("id/{id}")]
        public async Task<IActionResult> Delete([FromHeader] string idToken, Guid id)
        {
            var auth = await Utils.Authorization.Validate(idToken);
            if (auth == null) return Unauthorized("Authorization Token is Invalid.");

            var song = dbContext.Songs.Include(s => s.User).ToList().Find(s => s.Id == id);
            if (song == null)
                return BadRequest("Song doesn't exist");
            if (song.User.Email != auth.Email)
                return Unauthorized("You can't delete stuff from other users.");

            dbContext.Songs.Remove(song);
            dbContext.SaveChanges();

            return Ok(song);
        }

        [HttpPost("update/{songId}")]
        public async Task<IActionResult> Update([FromHeader] string idToken, SongDto songDto, Guid songId)
        {
            var auth = await Utils.Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var user = dbContext.Users.Find(auth.Email);
            if (user == null)
                return BadRequest("User doesn't exist.");

            var foundSong = dbContext.Songs.Find(songId);

            if (foundSong == null)
                return BadRequest("Song doesn't exist.");

            if (foundSong.User.Email != auth.Email)
                return BadRequest("You can't edit data from other users.");

            foundSong.Name = songDto.Name;
            foundSong.Lyrics = songDto.Lyrics;
            foundSong.Description = songDto.Description;
            foundSong.AccessFor = songDto.AccessFor;
            foundSong.LastUpdateDate = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            return Ok(foundSong);
        }
    }
}
