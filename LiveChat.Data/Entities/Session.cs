using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Data.Entities
{
    public class Session
    {
        public Guid Id { get; set; }

        public string ClientName { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }

        public Guid WebsiteId { get; set; }
        public Website Website { get; set; }

        public IList<ChatLog> ChatLogs { get; set; }
    }
}
