using LiveChat.Data.Entities;
using LiveChat.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveChat.Data.MockRepositories
{
    public class WebsiteMockRepository : IWebsiteRepository
    {
        List<Website> websites = new List<Website>()
        {
            new Website
            {
                Id = new Guid("8dea0b6d-c6cc-4189-acde-eada87c16b9a"),
                WebsiteUrl  = "www.aboba.com"
            }
        };
        public IList<Website> GetAllWebsites()
        {
            return websites;
        }

        public Website GetById(Guid id)
        {
            return websites.SingleOrDefault(x=>x.Id==id);
        }

        public Website Add(Website user)
        {
            throw new NotImplementedException();
        }
        public Website Edit(Website user)
        {
            throw new NotImplementedException();
        }
        public void Delete(Website user)
        {
            throw new NotImplementedException();
        }

        public Website GetByUrl(string url)
        {
            throw new NotImplementedException();
        }
    }
}
