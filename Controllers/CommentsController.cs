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
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public CommentsController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("GetBySongId/{songId}")]
        public async Task<IActionResult> GetById([FromHeader] string idToken, Guid songId)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var user = Misc.getUserByEmail(dbContext, auth.Email);
            if (user == null)
                return NotFound("User doesn't exist.");

            var comments = dbContext.Comments.Include(c => c.User).Include(c => c.Song).Where(c => c.Song.Id == songId).Select(c => Misc.CommentToPublic(c));

            return Ok(comments);
        }


        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromHeader] string idToken, DtoCommentAdd comment)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var user = Misc.getUserByEmail(dbContext, auth.Email);
            if (user == null)
                return NotFound("User doesn't exist.");

            var song = dbContext.Songs.Find(comment.SongId);
            if (song == null)
                return NotFound("Song doesn't exist");

            if (comment.Content.Replace(" ", "").Length < 2)
                return BadRequest("Comment must be at least 2 characters long without whitespace.");

            var newComment = new Comment
            {
                Id = new Guid(),
                User = user,
                Song = song,
                CreationDate = DateTime.UtcNow,
                Content = comment.Content,
                Edited = false
            };

            dbContext.Comments.Add(newComment);
            dbContext.SaveChanges();

            return Ok(Misc.CommentToPublic(newComment));
        }

        [HttpDelete("Delete/{commentId}")]
        public async Task<IActionResult> Delete([FromHeader] string idToken, Guid commentId)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var user = Misc.getUserByEmail(dbContext, auth.Email);
            if (user == null)
                return NotFound("User doesn't exist.");

            var comment = dbContext.Comments.Include(c => c.User).Include(c => c.Song).Where(c => c.Id == commentId).First();
            if (comment == null)
                return NotFound("Comment doesn't exist.");

            if (comment.User.Email != user.Email)
                return Unauthorized("You can't delete comments from other users.");

            dbContext.Comments.Remove(comment);
            dbContext.SaveChanges();

            return Ok(Misc.CommentToPublic(comment));
        }
    }
}
