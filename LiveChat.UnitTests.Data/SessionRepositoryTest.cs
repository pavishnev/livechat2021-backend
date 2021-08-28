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
   public class SessionRepositoryTest:IDisposable
    {
        private readonly LiveChatDbContext _context;

        public SessionRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<LiveChatDbContext>();
            options.UseInMemoryDatabase("SessionRepositoryTestInMemoryDatabase");

            _context = new LiveChatDbContext(options.Options);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        [Fact]
        public void GetAllNotEndedSessions()
        {
            //arrange
            var sessionEnded = new Session()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                EndedAt=DateTime.Now
            };
            var sessionNotEnded = new Session()
            {
                Id = new Guid("83A9E26C-0000-4821-B4A3-56CA06EBF4C7")
            };
            _context.Sessions.AddRange(sessionEnded,sessionNotEnded);
            _context.SaveChanges();
            var repository = new SessionRepository(_context);

            // Act
            var result = repository.GetNotEndedSessions();
            // Assert
            Assert.Equal(sessionNotEnded.Id, result[0].Id);

        }
        [Fact]
        public void GetAllEndedSessions()
        {
            //arrange
            var sessionEnded = new Session()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                EndedAt = DateTime.Now
            };
            var sessionNotEnded = new Session()
            {
                Id = new Guid("83A9E26C-0000-4821-B4A3-56CA06EBF4C7")
            };
            _context.Sessions.AddRange(sessionEnded, sessionNotEnded);
            _context.SaveChanges();
            var repository = new SessionRepository(_context);

            // Act
            var result = repository.GetEndedSessions();
            // Assert
            Assert.Equal(sessionEnded.Id, result[0].Id);

        }
        [Fact]
        public void GetWithStartEndDate()
        {
            //arrange

            var sessionOne = new Session()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                EndedAt = DateTime.Now.AddDays(1),
                StartedAt=DateTime.Now.AddDays(-1)
            };
            var session = new Session()
            {
                Id = new Guid("83A9E26C-0000-4821-B4A3-56CA06EBF4C7"),
                EndedAt = DateTime.Now.AddDays(1),
                StartedAt = DateTime.Now.AddDays(-1)
            };
            _context.Sessions.AddRange(sessionOne, session);
            _context.SaveChanges();
            var repository = new SessionRepository(_context);

            // Act
            var result = repository.GetWithinStartEndDate(DateTime.Now);
            // Assert
            Assert.Equal(2, result.Count());

        }

 
        [Fact]
        public void GetById()
        {
            //arrange
            var session = new Session()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7")
            };
            _context.Sessions.AddRange(session);
            _context.SaveChanges();
            var Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7");
            var repository = new SessionRepository(_context);

            // Act
            var result = repository.GetById(Id);
            // Assert
            Assert.Equal(Id, result.Id);

        }
  

        [Fact]
        public void Add()
        {
            //arrange
            var session = new Session()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7")
            };

            var repository = new SessionRepository(_context);

            // Act
            var result = repository.Add(session);
            // Assert
            Assert.Equal(1, _context.Sessions.Count());

        }
        [Fact]
        public void Edit()
        {
            //arrange
            var sessionOld = new Session()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
             ClientName ="Old"
            };
            var sessionNew = new Session()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                ClientName = "New"
            };

            _context.Sessions.AddRange(sessionOld);
            _context.SaveChanges();
            var repository = new SessionRepository(_context);

            // Act
            var result = repository.Edit(sessionNew);
            // Assert
            Assert.Equal(sessionNew.ClientName, result.ClientName);

        }
        [Fact]
        public void Delete()
        {
            //arrange
            var session = new Session()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7")
            };

            _context.Sessions.AddRange(session);
            _context.SaveChanges();
            var repository = new SessionRepository(_context);

            // Act
            repository.Delete(session);
            // Assert
            Assert.Equal(0, _context.Sessions.Count());

        }
    }
}
