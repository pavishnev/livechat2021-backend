using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Business.Models.RESTResponces
{
    public class AgentViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public int CompletedChatsCount { get; set; }
        public Guid? InvitationCode { get; set; }
    }
}
