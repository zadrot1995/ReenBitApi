using Microsoft.AspNetCore.Identity;
using ReenbitTest2.DbContexts;
using ReenbitTest2.Dto;
using ReenbitTest2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReenbitTest2.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext dbContext;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.dbContext = dbContext;
        }
        public async Task<UserDto> Register(RegisterViewModel model)
        {
            User user = new User
            {
                Email = model.Name.Trim(),
                UserName = model.Name.Trim(),
                EmailConfirmed = true
            };
            await dbContext.SaveChangesAsync();
            var isCreated = await _userManager.CreateAsync(user, model.Password);
            if (isCreated.Succeeded)
            {
                var result =
                await _signInManager.PasswordSignInAsync(model.Name, model.Password, true, false);
                if (result.Succeeded)
                {
                    UserDto userDto = new UserDto { Id = user.Id, Name = model.Name };
                    return userDto;
                }
            }
            return null;
        }

        public async Task<UserDto> Login(LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Name);
            if (user == null)
            {
                return null;
            }
            var isIdentical = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isIdentical)
            {
                return null;
            }

            var result =
                await _signInManager.PasswordSignInAsync(model.Name, model.Password, true, false);
            if (result.Succeeded)
            {
                var responce = new UserDto
                {
                    Id = user.Id,
                    Name = user.UserName
                };

                return responce;
            }
            return null;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
