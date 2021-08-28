using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Business.Models
{
    public class ChatModel
    {
        public List<MessageModel> Messages { get; set; }
        public ClientModel User { get; set; }
        public ChatModel()
        {
            Messages = new List<MessageModel>();
            User = new ClientModel();
        }
    }
}
