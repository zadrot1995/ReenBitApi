using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DbContexts;
using Api.Dto;
using Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext dbContext;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _signInManager.UserManager.Users.ToListAsync();
        }


        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Email = model.Name,
                    UserName = model.Name,
                    
                };
                await dbContext.SaveChangesAsync();
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
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
                var user = await _userManager.FindByNameAsync(model.Name);
                if (user == null)
                {
                    return BadRequest();
                }

                var isIdentical = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!isIdentical)
                {
                    return BadRequest();
                }

                var result =
                    await _signInManager.PasswordSignInAsync(model.Name, model.Password, false, false);
                if (result.Succeeded && await _userManager.IsEmailConfirmedAsync(await _userManager.FindByEmailAsync(model.Name)))
                {
                   
                    var responce = new UserDto
                    {
                        Id = user.Id,
                        Name = user.UserName
                    };

                    return Ok(responce);
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return Unauthorized();
        }
        

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var a = this.User;
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return Ok();
        }
      

    }
}
