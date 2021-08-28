using System;
using System.Collections.Generic;

namespace LiveChat.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] Salt { get; set; }

        //foreign keys
        public Guid WebsiteId { get; set; }
        public Website Website { get; set; }

        public IList<PasswordChangeToken> PasswordChangeTokens { get; set; }
        public IList<ChatLog> ChatLogs { get; set; }
    }
}
