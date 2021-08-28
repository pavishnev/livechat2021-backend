using LiveChat.Data.Entities;
using LiveChat.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Data.Repositories
{
    public class ChatLogRepository : IChatLogRepository
    {
        private LiveChatDbContext _context;
        public ChatLogRepository(LiveChatDbContext context)
        {
            _context = context;
        }
        public IList<ChatLog> GetAllChatLogs() => _context.ChatLogs.AsNoTracking().ToList();

        public IList<ChatLog> GetByAgentId(Guid agentId) => _context.ChatLogs.AsNoTracking().Where(x=>x.UserId==agentId).Include(x=>x.Session).ToList();

        public IList<ChatLog> GetBySessionId(Guid sessionId) => _context.ChatLogs.AsNoTracking().Where(x => x.SessionId == sessionId).ToList();
        public ChatLog GetById(Guid id) => _context.ChatLogs.AsNoTracking().SingleOrDefault(x => x.Id == id);
        public ChatLog Add(ChatLog chatlog)
        {
            chatlog.Id = Guid.NewGuid();
            _context.ChatLogs.Add(chatlog);
            _context.SaveChanges();
            return chatlog;
        }
        public ChatLog Edit(ChatLog chatlog)
        {
            var toEdit = _context.ChatLogs.SingleOrDefault(c => c.Id == chatlog.Id);
            if (toEdit != null)
            {
                toEdit.Message = chatlog.Message;
                toEdit.IsSentByClient = chatlog.IsSentByClient;
                toEdit.Timestamp = chatlog.Timestamp;
                toEdit.UserId = chatlog.UserId;
                toEdit.SessionId = chatlog.SessionId;
                _context.ChatLogs.Update(toEdit);
                _context.SaveChanges();
            }
            return toEdit;
        }
        public void Delete(ChatLog chatlog)
        {
            _context.ChatLogs.Remove(chatlog);
            _context.SaveChanges();
        }
    }
}
