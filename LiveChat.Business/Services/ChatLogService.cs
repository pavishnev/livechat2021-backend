using LiveChat.Business.Models.RESTResponces;
using LiveChat.Business.Services.Interfaces;
using LiveChat.Data.Entities;
using LiveChat.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Business.Services
{
    public class ChatLogService : IChatLogService
    {
        private readonly IChatLogRepository _chatLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;

        public ChatLogService(
            IChatLogRepository chatLogRepository,
            IUserRepository userRepository,
            ISessionRepository sessionRepository)
        {
            _chatLogRepository = chatLogRepository;
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
        }

        public List<ChatLogViewModel> GetChatLog(Guid sessionId)
        {
            List<ChatLog> chatLog = _chatLogRepository
                    .GetBySessionId(sessionId)
                    .ToList();

            List<ChatLogViewModel> chatLogs = new();

            foreach (ChatLog message in chatLog)
            {
                ChatLogViewModel chatLogViewModel = new ChatLogViewModel()
                {
                    Text = message.Message,
                    IsSentByClient = message.IsSentByClient,
                    Timestamp = message.Timestamp,
                };

                chatLogs.Add(chatLogViewModel);
            }

            return chatLogs;
        }

        public int GetSessionsCount(Guid websiteId)
        {
           return _sessionRepository.GetAllSessions(websiteId).Where(x => x.ChatLogs.Count > 0).Count();
        }

        public List<SessionViewModel> GetSessionsPageByPage(Guid websiteId, int page, int elementsPerPage)
        {
            List<SessionViewModel> sessions = new();
         
            var sessionsList = _sessionRepository.GetAllSessions(websiteId).Where(x=>x.ChatLogs.Count>0).ToList();

            int firstIndex = (page - 1) * elementsPerPage;
            int lastIndex = (firstIndex + elementsPerPage) < sessionsList.Count ? (firstIndex + elementsPerPage) : sessionsList.Count;

            for (int i = firstIndex; i < lastIndex; i++)
            {
                sessions.Add(new SessionViewModel() {
                    Id = sessionsList[i].Id,
                    AgentName = sessionsList[i].ChatLogs.FirstOrDefault().User.Name,
                    ClientName = sessionsList[i].ClientName,
                    StartedAt = sessionsList[i].StartedAt,
                    EndedAt = sessionsList[i].EndedAt,
                });
            }

            return sessions;
        }
    }
}
