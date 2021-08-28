using LiveChat.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Data.Repositories.Interfaces
{
    public interface ISessionRepository
    {
     
            Session GetById(Guid id);
            IList<Session> GetAllSessions(Guid websiteId);
            IList<Session> GetNotEndedSessions();
            IList<Session> GetEndedSessions();
        //Получить сессии, где указанная дата попадает в рамки начала-конца сессии
          IList<Session> GetWithinStartEndDate(DateTime datetime);
            Session Add(Session session);
            Session Edit(Session session);
            void Delete(Session session);
    }
}
