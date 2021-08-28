using System;

namespace LiveChat.Business.Models.RESTResponces
{
    public class ChatLogViewModel
    {
        public string Text { get; set; }
        public bool IsSentByClient { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
