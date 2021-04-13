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
    }
}
