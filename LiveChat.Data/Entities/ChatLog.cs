using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Data.Entities
{
    public class ChatLog
    {
        public Guid Id { get; set; }

        public string Message { get; set; }
        public bool IsSentByClient { get; set; }
        public DateTime Timestamp { get; set; }

        //foreign keys
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid SessionId { get; set; }
        public Session Session { get; set; }
    }
}
