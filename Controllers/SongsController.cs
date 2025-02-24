using Google.Apis.Auth;
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
    public class SongsController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public SongsController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //[HttpGet("GetAll")]
        //public IActionResult GetAll()
        //{
        //    var songs = dbContext.Songs.Include(s => s.User);
        //    return Ok(songs);
        //}

        [HttpGet("GetPublic")]
        public async Task<IActionResult> GetPublic([FromHeader] string idToken)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var songs = dbContext.Songs
                .Include(s => s.User)
                .Where(s => s.AccessFor == 0)
                .Select(s => Misc.SongToPublic(dbContext, s));

            if (songs == null)
                return BadRequest("Database issue.");

            return Ok(songs);
        }


        [HttpGet("GetByUser")]
        public async Task<IActionResult> GetByUser([FromHeader] string idToken)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var songs = dbContext.Songs.Include(s => s.User).Where(s => s.User.Email == auth.Email);

            if (songs == null)
                return BadRequest("idk why");

            return Ok(songs);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById([FromHeader] string idToken, Guid id)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var song = dbContext.Songs.Include(s => s.User).ToList().Find(s => s.Id == id);
            if (song == null)
                return BadRequest("Song doesn't exist");

            if (song.User.Email != auth.Email)
                return Unauthorized("You can't fetch private data from other users.");

            return Ok(song);
        }

        [HttpGet("GetByIdPublic/{id}")]
        public async Task<IActionResult> GetByIdPublic([FromHeader] string idToken, Guid id)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var song = dbContext.Songs.Include(s => s.User).ToList().Find(s => s.Id == id);
            if (song == null)
                return BadRequest("Song doesn't exist");

            var songPublic = Misc.SongToPublic(dbContext, song);

            return Ok(songPublic);
        }

        [HttpGet("GetByUserId/{userId}")]
        public async Task<IActionResult> GetByUserId([FromHeader] string idToken, Guid userId)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var connectedUser = Misc.getUserByEmail(dbContext, auth.Email);

            var songs = dbContext.Songs
                .Include(s => s.User)
                .Where(s => s.AccessFor == TAccessFor.Public && s.User.Id == userId);


            var songsPublic = songs.Select(s => Misc.SongToPublic(dbContext, s));

            return Ok(songsPublic);
        }


        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromHeader] string idToken, DtoSongAdd songDto)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var user = Misc.getUserByEmail(dbContext, auth.Email);
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

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete([FromHeader] string idToken, Guid id)
        {
            var auth = await Authorization.Validate(idToken);
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

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromHeader] string idToken, DtoSongUpdate songDto)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var user = Misc.getUserByEmail(dbContext, auth.Email);
            if (user == null)
                return BadRequest("User doesn't exist.");

            var foundSong = dbContext.Songs.Find(songDto.Id);

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
