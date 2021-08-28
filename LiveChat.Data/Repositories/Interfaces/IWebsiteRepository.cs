using LiveChat.Data.Entities;
using System;
using System.Collections.Generic;

namespace LiveChat.Data.Repositories.Interfaces
{
    public interface IWebsiteRepository
    {
        Website GetById(Guid id);

        Website GetByUrl(string url);
        IList<Website> GetAllWebsites();
        
        Website Add(Website user);
        Website Edit(Website user);
        void Delete(Website user);
    }
}
