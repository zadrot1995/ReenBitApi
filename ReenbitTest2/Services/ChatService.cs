using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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

        public IEnumerable<string> GetConnectionsFromUser(IEnumerable<User> users)
        {
            List<string> chatConnections = new List<string>();

            foreach (var u in users)
            {
                var connections = dbContext.UserConnections.Include(x => x.User).Where(x => x.UserId == u.Id);
                if (connections != null)
                {
                    foreach (var connection in connections)
                    {
                        chatConnections.Add(connection.ConnectionString);

                    }
                }
            }
            return chatConnections;
        }

        public async Task CreateUserConnection(HubCallerContext context , UserConnectDto userConnectDto)
        {
            var user = await _signInManager.UserManager.Users.Where(x => x.Id == userConnectDto.UserId).Include(x => x.ConnectionStrings).FirstOrDefaultAsync();

            if (user != null && await dbContext.UserConnections.Where(x => x.ConnectionString == context.ConnectionId).FirstOrDefaultAsync() == null)
            {
                user.ConnectionStrings.Add(new UserConnection { UserId = user.Id, User = user, ConnectionString = context.ConnectionId });
                await dbContext.SaveChangesAsync();
            }

            else
            {
                throw new Exception("The user is not exist!");
            }
        }

    }
}
