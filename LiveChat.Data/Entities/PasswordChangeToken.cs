using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveChat.Data.Entities
{
    public class PasswordChangeToken
    {
        public Guid Id { get; set; }
        
        public DateTime ExpirationDate { get; set; }
        public bool IsExpired { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
