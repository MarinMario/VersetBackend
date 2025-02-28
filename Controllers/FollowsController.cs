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
    public class FollowsController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public FollowsController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("GetFollowing")]
        public async Task<IActionResult> GetFollowing([FromHeader] string idToken)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var currentUser = Misc.getUserByEmail(dbContext, auth.Email);
            if (currentUser == null)
                return NotFound("Connected User doesn't exist.");

            var follows = dbContext.Follows
                .Include(f => f.User)
                .Where(f => f.User.Id == currentUser.Id)
                .Select(f => f.Follows.Id);

            return Ok(follows);
        }

        [HttpGet("GetFollowStatus/{userId}")]
        public async Task<IActionResult> GetFollowStatus([FromHeader] string idToken, Guid userId)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var currentUser = Misc.getUserByEmail(dbContext, auth.Email);
            if (currentUser == null)
                return NotFound("Connected User doesn't exist.");

            var followStatus = new DtoFollowStatus { FollowStatus = FollowStatusType.None };

            var follow = dbContext.Follows
                .Include(f => f.User).Include(f => f.Follows)
                .Where(f => f.User.Id == currentUser.Id && f.Follows.Id == userId)
                .FirstOrDefault();

            if (follow != null)
                followStatus.FollowStatus = follow.FollowStatus;

            return Ok(followStatus);
        }


        [HttpGet("GetFollowers")]
        public async Task<IActionResult> GetFollowRequests([FromHeader] string idToken)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var currentUser = Misc.getUserByEmail(dbContext, auth.Email);
            if (currentUser == null)
                return NotFound("Connected User doesn't exist.");

            var follows = dbContext.Follows
                .Include(f => f.User).Include(f => f.Follows)
                .Where(f => f.Follows.Id == currentUser.Id)
                .Select(f => Misc.FollowToPublic(f));

            return Ok(follows);
        }


        [HttpPost("AddFollowRequest/{wantsToFollowId}")]
        public async Task<IActionResult> AddFollowRequest([FromHeader] string idToken, Guid wantsToFollowId)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var currentUser = Misc.getUserByEmail(dbContext, auth.Email);

            if (currentUser == null)
                return NotFound("Connected User doesn't exist.");

            var wantsToFollow = dbContext.Users.Find(wantsToFollowId);
            if (wantsToFollow == null)
                return NotFound("Account that User wants to follow doesn't exist.");

            var newFollow = new Follow
            {
                Id = new Guid(),
                Follows = wantsToFollow,
                User = currentUser,
                FollowStatus = FollowStatusType.None,
                Date = DateTime.UtcNow,
            };


            if (wantsToFollow.Public)
            {
                newFollow.FollowStatus = FollowStatusType.Following;
            }
            else
            {
                newFollow.FollowStatus = FollowStatusType.Requested;
            }

            dbContext.Follows.Add(newFollow);

            dbContext.SaveChanges();
            return Ok(new DtoFollowStatus { FollowStatus = newFollow.FollowStatus });
        }

        [HttpPost("AcceptFollowRequest/{userId}")]
        public async Task<IActionResult> AcceptFollowRequest([FromHeader] string idToken, Guid userId)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var currentUser = Misc.getUserByEmail(dbContext, auth.Email);

            if (currentUser == null)
                return NotFound("Connected User doesn't exist.");

            var followRequest = dbContext.Follows
                .Include(f => f.User).Include(f => f.Follows)
                .Where(f => f.Follows.Id == currentUser.Id && f.User.Id == userId)
                .Where(f => f.FollowStatus == FollowStatusType.Requested)
                .FirstOrDefault();

            if (followRequest == null)
                return NotFound("Follow request doesn't exist.");

            followRequest.FollowStatus = FollowStatusType.Following;
            followRequest.Date = DateTime.UtcNow;
            dbContext.SaveChanges();

            return Ok();
        }

        [HttpDelete("Unfollow/{userId}")]
        public async Task<IActionResult> Unfollow([FromHeader] string idToken, Guid userId)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var currentUser = Misc.getUserByEmail(dbContext, auth.Email);

            if (currentUser == null)
                return NotFound("Connected User doesn't exist.");

            var follow = dbContext.Follows
                .Include(f => f.User).Include(f => f.Follows)
                .Where(f => f.User.Id == currentUser.Id && f.Follows.Id == userId)
                .FirstOrDefault();

            if (follow == null)
                return NotFound("You are not following that user.");

            dbContext.Follows.Remove(follow);
            dbContext.SaveChanges();

            return Ok();
        }


        [HttpDelete("DeleteFollower/{followerId}")]
        public async Task<IActionResult> DeleteFollower([FromHeader] string idToken, Guid followerId)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var currentUser = Misc.getUserByEmail(dbContext, auth.Email);

            if (currentUser == null)
                return NotFound("Connected User doesn't exist.");

            var follower = dbContext.Follows
                .Include(f => f.User).Include(f => f.Follows)
                .Where(f => f.Follows.Id == currentUser.Id && f.User.Id == followerId)
                .FirstOrDefault();

            if (follower == null)
                return NotFound("Follower doesn't exist.");

            dbContext.Follows.Remove(follower);
            dbContext.SaveChanges();

            return Ok();
        }
    }
}
