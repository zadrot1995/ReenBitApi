using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using ReenbitTest2.DbContexts;
using ReenbitTest2.Dto;
using ReenbitTest2.Hubs;
using ReenbitTest2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReenbitTest2.Services
{
    public class ChatService
    {
        private readonly IHubContext<ChatHub> hubContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext dbContext;

        public ChatService(IHubContext<ChatHub> hubContext, UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext dbContext)
        {
            this.hubContext = hubContext;
            _userManager = userManager;
            _signInManager = signInManager;
            this.dbContext = dbContext;
        }

        public async Task SendMessage(ChatMessage message)
        {
            var user = this._userManager.Users.Where(x => x.Id == message.To).FirstOrDefault();
            if (user != null)
            {
                //await this.hubContext.Clients.Clients(user.ConnectionStrings.Select(x => x.ConnectionString).ToList()).SendAsync("messageReceivedFromApi", message);
                await this.hubContext.Clients.All.SendAsync("messageReceivedFromApi", message);
            }
        }

        public async Task<NewConnectionDto> AddNewConnection(NewConnectionDto newConnectionDto)
        {
            var connections = dbContext.UserConnections.Where(x => x.UserId == newConnectionDto.Id);
            if (connections == null)
            {
                return null;
            }
            dbContext.UserConnections.Add(new Models.UserConnection { ConnectionString = newConnectionDto.ConnectionString, UserId = newConnectionDto.Id });
            await dbContext.SaveChangesAsync();
            return newConnectionDto;
        }

        public async Task<NewConnectionDto> DeleteConnection(NewConnectionDto newConnectionDto)
        {
            var user = this._userManager.Users.Where(x => x.Id == newConnectionDto.Id).FirstOrDefault();
            if (user != null)
            {
                dbContext.UserConnections.Remove(dbContext.UserConnections.Where(x => x.ConnectionString == newConnectionDto.ConnectionString).FirstOrDefault());
                await dbContext.SaveChangesAsync();
                return newConnectionDto;
            }
            return null;
        }

    }
}
