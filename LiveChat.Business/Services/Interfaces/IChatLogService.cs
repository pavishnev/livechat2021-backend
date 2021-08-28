using LiveChat.Business.Models.RESTResponces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Business.Services.Interfaces
{
    public interface IChatLogService
    {
        public List<SessionViewModel> GetSessionsPageByPage(Guid websiteId, int page, int elementsPerPage);
        public List<ChatLogViewModel> GetChatLog(Guid sessionId);
        public int GetSessionsCount(Guid websiteId);
    }
}
