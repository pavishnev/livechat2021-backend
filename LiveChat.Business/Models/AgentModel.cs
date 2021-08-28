using LiveChat.Data.Entities;
using System;
using System.Collections.Generic;

namespace LiveChat.Business.Models
{
    public class AgentModel
    {
       public Guid Id { get; set; }
        public string WebsiteId { get; set; }

        public List<Session> ClientsOnline { get; set; }

        public AgentModel()
        {
            ClientsOnline = new List<Session>();
        }

        public AgentModel(Guid id)
        {
            Id = id;
            ClientsOnline = new List<Session>();
        }
    }
}
