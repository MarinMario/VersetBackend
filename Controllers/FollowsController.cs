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

        [HttpGet("GetFollows")]
        public async Task<IActionResult> GetFollows([FromHeader] string idToken)
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
                .Select(f => f.FollowsId);

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

            var followRequest = dbContext.FollowRequests
                .Include(f => f.User)
                .Where(f => f.User.Id == currentUser.Id && f.FollowsId == userId)
                .First();

            if (followRequest != null)
            {
                followStatus.FollowStatus = FollowStatusType.Requested;
                return Ok(followStatus);
            }

            var follow = dbContext.Follows
                .Include(f => f.User)
                .Where(f => f.User.Id == currentUser.Id && f.FollowsId == userId)
                .First();

            if (follow != null)
                followStatus.FollowStatus = FollowStatusType.Following;

            return Ok(followStatus);
        }


        [HttpGet("GetFollowRequests")]
        public async Task<IActionResult> GetFollowRequests([FromHeader] string idToken)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var currentUser = Misc.getUserByEmail(dbContext, auth.Email);
            if (currentUser == null)
                return NotFound("Connected User doesn't exist.");

            var followRequests = dbContext.FollowRequests
                .Where(f => f.FollowsId == currentUser.Id)
                .Select(f => f.FollowsId);

            return Ok(followRequests);
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
                FollowsId = wantsToFollowId,
                User = currentUser,
            };

            var followStatus = new DtoFollowStatus { FollowStatus = FollowStatusType.None };

            if (wantsToFollow.Public)
            {
                dbContext.Follows.Add(newFollow);
                followStatus.FollowStatus = FollowStatusType.Following;
            }
            else
            {
                dbContext.FollowRequests.Add(newFollow);
                followStatus.FollowStatus = FollowStatusType.Requested;
            }

            dbContext.SaveChanges();
            return Ok(followStatus);
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

            var followRequest = dbContext.FollowRequests
                .Include(f => f.User)
                .Where(f => f.FollowsId == userId && f.User.Id == currentUser.Id)
                .First();

            if (followRequest == null)
                return NotFound("Follow request doesn't exist.");

            dbContext.Follows.Add(followRequest);
            dbContext.FollowRequests.Remove(followRequest);
            dbContext.SaveChanges();

            return Ok();
        }

        [HttpDelete("DeleteFollowRequest/{userId}")]
        public async Task<IActionResult> DeleteFollowRequest([FromHeader] string idToken, Guid userId)
        {
            var auth = await Authorization.Validate(idToken);
            if (auth == null)
                return Unauthorized("Authorization Token is Invalid.");

            var currentUser = Misc.getUserByEmail(dbContext, auth.Email);

            if (currentUser == null)
                return NotFound("Connected User doesn't exist.");

            var followRequest = dbContext.FollowRequests
                .Include(f => f.User)
                .Where(f => f.FollowsId == userId && f.User.Id == currentUser.Id)
                .First();

            if (followRequest == null)
                return NotFound("Follow request doesn't exist.");

            dbContext.FollowRequests.Remove(followRequest);
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
                .Include(f => f.User)
                .Where(f => f.FollowsId == currentUser.Id && f.User.Id == followerId)
                .First(); ;

            if (follower == null)
                return NotFound("Follower doesn't exist.");

            dbContext.Follows.Remove(follower);
            dbContext.SaveChanges();

            return Ok();
        }
    }
}
