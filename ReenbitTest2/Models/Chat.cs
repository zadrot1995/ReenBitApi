using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ReenbitTest2.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
        public List<ChatMessage> Messages { get; set; }
        public string ChatType { get; set; }

    }
}
