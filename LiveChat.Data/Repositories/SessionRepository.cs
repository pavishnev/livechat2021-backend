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
    public class SessionRepository : ISessionRepository
    {
        private LiveChatDbContext _context;
        public SessionRepository(LiveChatDbContext context)
        {
            _context = context;
        }
        public IList<Session> GetAllSessions(Guid websiteId) => 
            _context.Sessions.Where(x=>x.WebsiteId==websiteId)
            .Include(x=>x.ChatLogs)
            .ThenInclude(x=>x.User)
            .AsNoTracking()
            .ToList();

        public IList<Session> GetNotEndedSessions() { lock (_context.Sessions) { return _context.Sessions.AsNoTracking().Where(x => x.EndedAt == null).ToList(); } }
        public IList<Session> GetEndedSessions()=>  _context.Sessions.AsNoTracking().Where(x => x.EndedAt != null).ToList();
        public IList<Session> GetWithinStartEndDate(DateTime datetime) => _context.Sessions.AsNoTracking().Where(x => (x.EndedAt != null)&&(x.StartedAt<datetime)&&(x.EndedAt>datetime)).ToList();
        public Session GetById(Guid id) => _context.Sessions.AsNoTracking().SingleOrDefault(x => x.Id == id);

        public Session Add(Session session)
        {
            session.Id = Guid.NewGuid();
            _context.Sessions.Add(session);
            _context.SaveChanges();
            return session;
        }

        public Session Edit(Session session)
        {
            var toEdit = _context.Sessions.SingleOrDefault(c => c.Id == session.Id);
            if(toEdit!=null)
            {
                toEdit.ClientName = session.ClientName;
                toEdit.StartedAt = session.StartedAt;
                toEdit.EndedAt = session.EndedAt;
                toEdit.WebsiteId = session.WebsiteId;
                _context.Sessions.Update(toEdit);
                _context.SaveChanges();
            }
            return toEdit;
        }

        public void Delete(Session session)
        {
            _context.Sessions.Remove(session);
            _context.SaveChanges();
        }


     
    }
}
