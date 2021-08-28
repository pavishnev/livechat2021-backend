using System;
using System.Collections.Generic;

namespace LiveChat.Data.Entities
{
    public class Website
    {
        public Guid Id { get; set; }
        public string WebsiteUrl { get; set; }

        public IList<User> Users { get; set; }
        public IList<Session> Sessions { get; set; }
    }
}
