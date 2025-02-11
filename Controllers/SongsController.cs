using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VersuriAPI.Data;
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
        public IActionResult getAll()
        {
           var songs = dbContext.Songs.ToList();
             
            return Ok(songs);
        }

        [HttpGet("{id}")]
        public IActionResult get(Guid id)
        {
            var song = dbContext.Songs.Find(id);

            if(song == null)
            {
                return BadRequest("Song doesn't exist");
            }

            return Ok(song);
        }

        [HttpPost]
        public IActionResult add(Song song)
        {
            dbContext.Songs.Add(song);
            dbContext.SaveChanges();
            return Ok(song);
        }
    }
}
