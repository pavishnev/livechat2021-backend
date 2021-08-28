using System;

namespace LiveChat.Business.Models
{
    public class MessageModel
    {
        public string SessionId { get; set; }

        public string Text { get; set; }

        public bool IsSentByClient { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
