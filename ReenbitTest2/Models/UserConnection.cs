using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReenbitTest2.Models
{
    public class UserConnection
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string ConnectionString { get; set; }
    }
}
