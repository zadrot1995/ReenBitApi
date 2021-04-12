using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReenbitTest2.DbContexts;
using ReenbitTest2.Dto;
using ReenbitTest2.Models;
using ReenbitTest2.Services;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserService userService;

        public AccountController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        //[Authorize(Policy = "ForUser")]
        public async Task<ActionResult<string>> GetUsers()
        {
            var user = User;
            return User.Identity.Name;
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await userService.Register(model);
                if(result != null)
                {
                    return Ok(result);
                }
            }
            return BadRequest(model);
        }


        [HttpPost("login")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<UserDto>> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await userService.Login(model);
                if (result != null)
                {
                    return Ok(result);
                }
            }
            return Unauthorized();
        }
        

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await userService.Logout();
            return Ok();
        }

        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim("Type", "User")
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

    }
}
