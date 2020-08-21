using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Data;
using ServerApp.Dtos;
using ServerApp.Helpers;
using ServerApp.Models;

namespace ServerApp.Controllers
{   [ServiceFilter(typeof(LastActiveActionFilter))]
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ISocialRepository _repository;
        private readonly IMapper _mapper;
        public UsersController(ISocialRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;

        }

        public async Task<IActionResult> GetUsers([FromQuery] UserQueryParams userQueryParams)
        {
            userQueryParams.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            var users = await _repository.GetUsers(userQueryParams);
            var result = _mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repository.GetUser(id);
            var result = _mapper.Map<UserForDetailsDto>(user);
            return Ok(result);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto model)
        {
          if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return BadRequest("not valid request");

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _repository.GetUser(id);

            _mapper.Map(model,user);

            if (await _repository.SaveChanges())
                return Ok();

            throw new System.Exception("an error occured while updating please check and post again");
        }
        [HttpPost("{followerUserId}/follow/{userId}")]
        public async Task<IActionResult> FollowUser(int followerUserId,int userId)
        {
                if (followerUserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

                if(followerUserId == userId)
                return BadRequest("you can't follow yourself :/");

                var IsAlreadyFollowed = await _repository
                .IsAlreadyFollowed(followerUserId,userId);

                if(IsAlreadyFollowed)
                return BadRequest("you already follow the profile !, next other ");

                if(await _repository.GetUser(userId)== null)
                return NotFound();

                var follow =new UserToUser(){
                   
                   UserId = userId,
                   FollowerId =followerUserId
                };

                _repository.Add<UserToUser>(follow);

                if(await _repository.SaveChanges())

                return Ok();

                return BadRequest("en error occured please return back and try again later");

        }

        
    }
}