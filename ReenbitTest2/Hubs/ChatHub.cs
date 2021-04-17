using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ReenbitTest2.DbContexts;
using ReenbitTest2.Dto;
using ReenbitTest2.Models;
using ReenbitTest2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReenbitTest2.Hubs
{
    public class ChatHub : Hub<IChatHub>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ChatService chatService;

        public ChatHub(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext dbContext, ChatService chatService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.dbContext = dbContext;
            this.chatService = chatService;
        }

        public async Task BroadcastAsync(ChatMessage message)
        {
            List<string> chatConnections = new List<string>();

            var chat = dbContext.Chats
                .Include(x => x.Users)
                .Include(x => x.Messages)
                .Where(x => x.Id.ToString() == message.ChatId)
                .FirstOrDefault();
            chat.Messages.Add(message);
            await dbContext.SaveChangesAsync();
            if (chat != null)
            {
                await Clients.Clients(chatService.GetConnectionsFromUser(chat.Users).ToList()).MessageReceivedFromHub(message);
            }

        }
        public async Task CreateUserConnection(UserConnectDto userConnectDto)
        {
            try
            {
                await chatService.CreateUserConnection(Context, userConnectDto);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

       
        public async Task CreateChat(ChatCreateDto chatCreateDto)
        {
            if (chatCreateDto != null)
            {
                var chat = new Chat { Name = chatCreateDto.Name, Users = new List<User>(), ChatType = chatCreateDto.ChatType };
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
                var newChatDto = new ChatDto { Id = chat.Id.ToString(), ChatType = chatCreateDto.ChatType, Name = chat.Name };
                await Clients.Clients(chatService.GetConnectionsFromUser(chat.Users).ToList()).OnNewChatConected(newChatDto);
            }
        }


        public override async Task OnConnectedAsync()
        {
            await Clients.All.NewUserConnected("a new user connectd");
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }

    public interface IChatHub
    {
        Task MessageReceivedFromHub(ChatMessage message);

        Task OnNewChatConected(ChatDto chatDto);

        Task NewUserConnected(string message);
    }
    
}
