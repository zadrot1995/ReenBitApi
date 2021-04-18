using ReenbitTest2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReenbitTest2.Dto
{
    public class ChatCreateDto
    {
        public string Name { get; set; }
        public List<string> UsersId { get; set; }
        public string ChatType { get; set; }
        public string AdminId { get; set; }

    }
}
