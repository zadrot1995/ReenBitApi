using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ChatDto>>> GetChats(string userId)
        {
            if (userId != null)
            {
                var user = await _signInManager.UserManager.FindByIdAsync(userId);
                if (user != null)
                {
                    List<ChatDto> chatDtos = new List<ChatDto>();
                    var chats = await dbContext.Chats
                        .Include(x => x.Users)
                        .Include(x => x.Messages)
                        .Where(x => x.Users.Contains(user))
                        .ToListAsync();
                    foreach (var chat in chats)
                    {
                        chatDtos.Add(new ChatDto { Id = chat.Id.ToString(), Name = chat.Name, ChatType = chat.ChatType, AdminId = chat.AdminId});
                    }
                    return chatDtos;
                    
                }
                return NotFound();
            }
            return BadRequest();
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<Chat>> GetChatById(string Id)
        {
            if (Id != null)
            {
                var chat = await dbContext.Chats
                    .Include(x => x.Messages)
                    .Include(x => x.Users)
                    .Where(x => x.Id.ToString() == Id)
                    .FirstOrDefaultAsync();
                if (chat != null)
                {
                    return chat;
                }
                return NotFound();
            }
            return BadRequest();
        }


        [HttpPost]
        public async Task<IActionResult> CreateChat(ChatCreateDto chatCreateDto)
        {
            if (chatCreateDto != null)
            {
                var chat = new Chat { Name = chatCreateDto.Name, Users = new List<User>(), ChatType = chatCreateDto.ChatType, AdminId = chatCreateDto.AdminId };
                foreach (var userId in chatCreateDto.UsersId)
                {
                    var user = await _signInManager.UserManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        chat.Users.Add(user);
                    }
                }
                await dbContext.Chats.AddAsync(chat);
                await dbContext.SaveChangesAsync();
                var newChatDto = new ChatDto { Id = chat.Id.ToString(), ChatType = chatCreateDto.ChatType, Name = chat.Name, AdminId = chatCreateDto.AdminId };
                await hubContext.Clients.Clients(chatService.GetConnectionsFromUser(chat.Users).ToList()).SendAsync("OnNewChatCreated",
                    new ChatDto { Id = chat.Id.ToString(), ChatType = chatCreateDto.ChatType, Name = chat.Name });
                return Ok();
            }
            return BadRequest();
        }

       
        [HttpPost("disconnect")]
        public async Task<IActionResult> OnDisconnectedAsync(OnUserDisconnectDto onUserDisconnectDto)
        {
            if (onUserDisconnectDto != null)
            {
                var connection = dbContext.UserConnections.Where(c => c.ConnectionString == onUserDisconnectDto.ConnectionString).FirstOrDefault();
                if (connection != null)
                {
                    dbContext.UserConnections.Remove(connection);
                    await dbContext.SaveChangesAsync();
                    return Ok();
                }
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
