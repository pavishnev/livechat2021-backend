using System;

namespace LiveChat.Business.Models.RESTResponces
{
    public class SessionViewModel
    {
        public Guid Id { get; set; }
        public string AgentName { get; set; }
        public string ClientName { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
    }
}
