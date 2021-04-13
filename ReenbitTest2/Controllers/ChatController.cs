using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ReenbitTest2.DbContexts;
using ReenbitTest2.Dto;
using ReenbitTest2.Hubs;
using ReenbitTest2.Models;
using ReenbitTest2.Services;

namespace ReenbitTest2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> hubContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ChatService chatService;

        public ChatController(IHubContext<ChatHub> hubContext, UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext dbContext, ChatService chatService)
        {
            this.hubContext = hubContext;
            _userManager = userManager;
            _signInManager = signInManager;
            this.dbContext = dbContext;
            this.chatService = chatService;
        }



        [HttpPost]
        public async Task<IActionResult> CreateChat(ChatCreateDto chatCreateDto)
        {
            if(chatCreateDto != null)
            {
                var chat = new Chat { Name = chatCreateDto.Name, Users = new List<User>() };
                foreach(var userId in chatCreateDto.UsersId)
                {
                    var user = await _signInManager.UserManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        chat.Users.ToList().Add(user);
                    }
                }
                await dbContext.Chats.AddAsync(chat);
                await dbContext.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("newDevice")]
        public async Task<ActionResult> GetClient(NewConnectionDto newConnectionDto)
        {
            var result = await chatService.AddNewConnection(newConnectionDto);
            if(result != null)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("disconnect")]
        public async Task<IActionResult> OnDisconnected(NewConnectionDto newConnectionDto)
        {
            var result = await chatService.DeleteConnection(newConnectionDto);
            if (result != null)
            {
                return Ok();
            }
            return BadRequest();
        }


    }



    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Identity?.Name;
        }
    }
}
