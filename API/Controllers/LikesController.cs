using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {  
        private readonly IUserRepository _userRepositry;
        private readonly ILikesRepository _likesRepositry;
        public LikesController(IUserRepository userRepositry, ILikesRepository likesRepositry)
        {
            _likesRepositry = likesRepositry;
            _userRepositry = userRepositry;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username){
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepositry.GetUserByUsernameAsync(username);
            var sourceUser = await _likesRepositry.GetUserWithLikes(sourceUserId);

            if(likedUser == null) return NotFound();

            if(sourceUser.UserName==username) return BadRequest("You cannot like yourself!");

            var userLike = await _likesRepositry.GetUserLike(sourceUserId, likedUser.Id);

            if(userLike != null) return BadRequest("You already liked this user!");

            userLike = new UserLike{
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id,
            };

            sourceUser.LikedUsers.Add(userLike);

            if(await _userRepositry.SaveAllAsync()) return Ok();

            return BadRequest("Failed to like user");

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams){

            likesParams.UserId = User.GetUserId();
            var users =  await _likesRepositry.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }
    }
}