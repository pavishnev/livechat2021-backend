using LiveChat.Data.Entities;
using LiveChat.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveChat.Data.Repositories
{
    public class WebsiteRepository : IWebsiteRepository
    {
        private LiveChatDbContext _context;
        public WebsiteRepository(LiveChatDbContext context)
        {
            _context = context;
        }

        public IList<Website> GetAllWebsites() => _context.Websites.ToList();

        public Website GetById(Guid id) => _context.Websites.SingleOrDefault(x => x.Id == id);
        public Website Add(Website site)
        {
            site.Id = Guid.NewGuid();
            _context.Websites.Add(site);
            return site;
        }

        public Website Edit(Website site)
        {
            var toEdit = _context.Websites.SingleOrDefault(c => c.Id == site.Id);
            if (toEdit != null)
            {
                toEdit.WebsiteUrl = site.WebsiteUrl;
                _context.Websites.Update(toEdit);
                _context.SaveChanges();
            }
            return toEdit;
        }

        public void Delete(Website site)
        {
            _context.Websites.Remove(site);
            _context.SaveChanges();
        }

        public Website GetByUrl(string url) => _context.Websites.SingleOrDefault(c => c.WebsiteUrl == url);
    }
}
