using System;

namespace ReenbitTest2.Models
{
    public class ChatMessage
    {
        public string Text { get; set; }
        public string ConnectionId { get; set; }
        public DateTime DateTime { get; set; }
        public string To { get; set; }
    }
}
