using FakeItEasy;
using LiveChat.Data;
using LiveChat.Data.Entities;
using LiveChat.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace LiveChat.UnitTests.Data
{
    public class WebsiteRepositoryTest:IDisposable
    {
        private readonly LiveChatDbContext _context;

        public WebsiteRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<LiveChatDbContext>();
            options.UseInMemoryDatabase("WebsiteRepositoryTestInMemoryDatabase");

            _context = new LiveChatDbContext(options.Options);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void GetAllWebsites()
        {
            //arrange
            var count = 5;
            var fake = A.CollectionOfDummy<Website>(count);
            _context.Websites.AddRange(fake);
            _context.SaveChanges();
            var repository = new WebsiteRepository(_context);

            // Act
            var result = repository.GetAllWebsites();
            // Assert
            Assert.Equal(count, result.Count);

        }
   
        [Fact]
        public void GetById()
        {
            //arrange
            var website = new Website()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
            };
            _context.Websites.AddRange(website);
            _context.SaveChanges();
            var Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7");
            var repository = new WebsiteRepository(_context);

            // Act
            var result = repository.GetById(Id);
            // Assert
            Assert.Equal(Id, result.Id);

        }

        [Fact]
        public void Add()
        {
            //arrange
            var website = new Website()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
            };

            var repository = new WebsiteRepository(_context);

            // Act
            var result = repository.Add(website);
            // Assert
            Assert.Equal(website.Id, result.Id);

        }

        [Fact]
        public void Edit()
        {
            //arrange
            var websiteOld = new Website()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
            WebsiteUrl="Old"
            };
            var websiteNew = new Website()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                WebsiteUrl= "New"
            };

            _context.Websites.AddRange(websiteOld);
            _context.SaveChanges();
            var repository = new WebsiteRepository(_context);

            // Act
            var result = repository.Edit(websiteNew);
            // Assert
            Assert.Equal(websiteNew.WebsiteUrl, result.WebsiteUrl);

        }

        [Fact]
        public void Delete()
        {
            //arrange
            var website = new Website()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7")
            };

            _context.Websites.AddRange(website);
            _context.SaveChanges();
            var repository = new WebsiteRepository(_context);

            // Act
            repository.Delete(website);
            // Assert
            Assert.Equal(0, _context.Websites.Count());
        }
    }
}
