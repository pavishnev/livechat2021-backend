using LiveChat.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Data.Repositories.Interfaces
{
    public interface IChatLogRepository
    {
        ChatLog GetById(Guid id);
        IList<ChatLog> GetAllChatLogs();
        IList<ChatLog> GetBySessionId(Guid sessionId);
        IList<ChatLog> GetByAgentId(Guid agentId);
        ChatLog Add(ChatLog chatlog);
        ChatLog Edit(ChatLog chatlog);
        void Delete(ChatLog chatlog);
    }
}
