using Microsoft.AspNetCore.SignalR;
using ReenbitTest2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReenbitTest2.Hubs
{
    public class ChatHub : Hub<IChatHub>
    {
        public async Task BroadcastAsync(ChatMessage message)
        {
            await Clients.All.MessageReceivedFromHub(message);
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

        Task NewUserConnected(string message);
        Task UserDisconnected(string connectionId);
    }

}
