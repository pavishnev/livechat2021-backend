using LiveChat.Business.Services.Interfaces;
using LiveChat.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Business.Services
{
   public class SessionsControl: ISessionsControl
    {
        private readonly ISessionService _sessionService;
        public readonly ISessionRepository _sessionRepository;
        public SessionsControl(ISessionService sessionService, ISessionRepository sessionRepository)
        {
            _sessionService = sessionService;
            _sessionRepository = sessionRepository;
        }
        public async Task<bool> Run()
        {
            var notFinishedSessions = _sessionRepository.GetNotEndedSessions();
            TimeSpan period = new TimeSpan(24, 0, 0);
            foreach (var session in notFinishedSessions)
            {
                if (DateTime.Now - session.StartedAt >= period)
                {
                    _sessionService.DisconnectClient(session.Id);
                }
            }
            return true;
        }
    }
}
