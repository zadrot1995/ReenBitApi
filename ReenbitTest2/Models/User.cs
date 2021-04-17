using Microsoft.AspNetCore.Identity;
using ReenbitTest2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReenbitTest2.Models
{
    public class User: IdentityUser
    {
        public List<UserConnection> ConnectionStrings { get; set; }
        public List<Chat> Chats { get; set; }
    }
}
