using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ReenbitTest2.DbContexts;
using ReenbitTest2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReenbitTest2.Hubs
{
    public class ChatHub : Hub<IChatHub>
    {
        static readonly Dictionary<string, string> Users = new Dictionary<string, string>();
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext dbContext;

        public ChatHub(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.dbContext = dbContext;
        }
        public async Task Send(ChatMessage message)
        {
            await Clients.All.MessageReceivedFromHub(message);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.NewUserConnected("a new user connectd");
        }
        //public async Task OnConnectedAsync(UserConnectDto userConnectDto)
        //{

        //    var chat = dbContext.Chats.Where(x => x.Id.ToString() == userConnectDto.ChatId).FirstOrDefault();
        //    var user = await _signInManager.UserManager.FindByIdAsync(userConnectDto.UserId);
        //    user.ConnectionStrings.Add(new UserConnection { UserId = user.Id, User = user, ConnectionString = Context.ConnectionId });

        //    List<string> connections = new List<string>();
        //    foreach(var u in chat.Users)
        //    {
        //        foreach(var connection in u.ConnectionStrings.Select(x => x.ConnectionString ))
        //        {
        //            connections.Add(connection);
        //        }
        //    }
        //    await Clients.Clients(connections).SendAsync("NewUserConnected", user.UserName);
        //}

        //public async Task Leave(string username)
        //{
        //    Users.Remove(username);
        //    await Clients.All.SendAsync("", username);
        //}

        //public async Task Send(string username, string message, string chatId)
        //{
        //    List<string> connections = new List<string>();
        //    var chat = dbContext.Chats.Include(x=>x.Users).Where(x => x.Id.ToString() == chatId).FirstOrDefault();
        //    if (chat != null)
        //    {
        //        foreach (var u in chat.Users)
        //        {
        //                connections.Add(dbContext.UserConnections.Where(x => x.UserId == u.Id).Select(x => x.ConnectionString).FirstOrDefault());
        //        }
        //        await Clients.Clients(connections).SendAsync("MessageReceivedFromHub", username, message);

        //    }
        //}
    }

    public interface IChatHub
    {
        Task MessageReceivedFromHub(ChatMessage message);

        Task NewUserConnected(string message);
    }
}
