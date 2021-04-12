using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReenbitTest2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReenbitTest2.DbContexts
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
