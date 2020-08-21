using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServerApp.Dtos;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;

        public IConfiguration _configuration { get; }

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new User
            {
                Name = userDto.Name,
                UserName = userDto.UserName,
                Email = userDto.Email,
                Gender = userDto.Gender,
                Created = DateTime.Now,
                LastActive = DateTime.Now,
                DataOfBirth = userDto.DateOfBirth,
                City = userDto.City,
                Country =userDto.Country
                
                

            };
            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (result.Succeeded)
            {
                return StatusCode(201);
            }
            return BadRequest(result.Errors);

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return BadRequest(new { message = "User Name is doğru değil canim" });

            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded)
            {
                //login
                return Ok(new
                {
                    token = GenerateJwtToken(user),
                });
            }
            return Unauthorized();


        }



        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.UserName)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)


            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }



    }
}
