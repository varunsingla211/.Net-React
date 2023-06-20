using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AcountController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly TokenService tokenService;

        public AcountController(UserManager<AppUser> userManager, TokenService tokenService)
        {

            this.userManager = userManager;
            this.tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Unauthorized();
            var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (result)
            {
                return new UserDTO
                {
                    DisplayName = user.DisplayName,
                    Image = null,
                    Token = tokenService.CreateToken(user),
                    Username = user.UserName
                };
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if(userManager.Users.Any(x=>x.UserName == registerDTO.Username))
            {
                return BadRequest("Username already taken");
            }
            var user = new AppUser
            {
                DisplayName = registerDTO.DisplayName,
                Email = registerDTO.Email,
                UserName = registerDTO.Email
            };
            var result = userManager.CreateAsync(user, registerDTO.Password);
            if (result.IsCompletedSuccessfully)
            {
                return new UserDTO
                {
                    DisplayName = user.DisplayName,
                    Image = null,
                    Token = tokenService.CreateToken(user),
                    Username = user.UserName,
                };
            }
            return BadRequest("Problem registering user"); 
        }

    }
}
